"""
v1.1.0
    - Add rotation key only at frame 1 on face joints
    - Add key at first frame on all keyable channel
    - Segment scale compensate
    - Bake blendshape of every single frame for Unity console avatar
    - Export Beard blendshape for unity console
    - Data format
        Unity - console : animation - fbx, facial anim - json
        Unity - mobile : animation - fbx, facial anim - json
        Playcanvas - console : body & facial anim - fbx
        Playcanvas - mobile : animation - fbx, facial anim - json
    - Imports references
    - Bakes bind joints and face texture animation
    - Runs euler filter on bind joints
    - Cleans scene:
        - Removes namespaces
        - Deletes everything except the bind group
        - Deletes all static channels
        - Optimizes scene size
    - Exports FBX and JSON file with face animation values in the selected location
"""


from PySide2 import QtCore
from PySide2 import QtWidgets
from shiboken2 import wrapInstance
import maya.OpenMayaUI as omui
from sys import platform, path
import maya.cmds as cmds
import maya.mel as mel
from json import dump
import subprocess
import difflib
import os


BAKE_KW = {
    "TEXTURE_LOOKUP": {
        0: "cheerful",
        1: "win",
        2: "lose",
        3: "idle",
        4: "shocked",
        5: "cheeky",
        6: "thinking",
        7: "determined",
        8: "scared",
        9: "angry",
        10: "compression",
        11: "idle-blink",
    },
    "CLEAN_AVATAR_NODES": [
        "PROXY",
        "*TRS0001_CON",
        "*:TRS0001_CON",
        "*extras_GRP",
        "*outfits_GRP",
        "*outfit_GRP",
        "*hair_GRP",
        "C_face_GRP",
    ],
    "OPTIMIZE_OPTION": [
        "nurbsSrf",
        "sets",
        "partition",
        "transform",
        "displayLayer",
        "renderLayer",
        "animationCurve",
        "clip",
        "pose",
        "nurbsCrv",
        "unusedNurbsSrf",
        "cached",
        "deformer",
        "unusedSkinInfs",
        "expression",
        "groupIDn",
        "shader",
        "locator",
        "ptCon",
        "pb",
        "snapshot",
        "unitConversion",
        "referenced",
        "brush",
        "unknownNodes",
        "shadingNetworks",
    ],
    "UNLOCK_NODES": ["JUNK", "BAKE", "CAMERA"],
    "REMOVE_BIND": "|AVATAR|bind_GRP",
    "FACIAL_JSON": {"playcanvas": "faceAnim.json"},
    "BIND_GRP": {"mobile": ("*:*:*bind_JNT", "*:*bind_JNT")},
    "FACE_SHAPES": {"mobile": ("*:*C_head_polyMove", "C_head_polyMove")},
    "BAKE_GRP": ("*:*bind_GRP", "*:*geo_GRP", "*bind_GRP", "BAKE"),
    "REMOVE_NAMESPACE": "*:*AVATAR",
    "FACE_ANIM_CONTROL": "C_head_CON",
    "FACE_ANIM_ATTR": "faceShapes",
    "REF_KW": ["snap_console_rig", "anim_rig", "console_rig"],
    "PROP_ROOT_JNT": "C_base_bind_JNT",
    "FACEBONE_EXCLUDE": [
        "C_jaw_base_JNT",
        "R_eye_rig_base_JNT",
        "R_eyeBrow_bind_JNT",
        "L_eye_rig_base_JNT",
        "L_eyeBrow_bind_JNT",
    ],
    "attrT": ["tx", "ty", "tz"],
    "attrR": ["rx", "ry", "rz"],
    "attrS": ["sx", "sy", "sz"],
}
# for facial animation
key_anim_dict = {}
key_anim_temp = {}
facialhair_dict = {}
root_path = "geo_GRP/head_GRP/"
beard_path = "geo_GRP/facial_hair_GRP/"
shape_list = {
    "face_shapes": {"name": "face_shapes", "path": (root_path + "C_head_GEO")},
    "eyelash_shapes": {
        "name": "eyelash_shapes",
        "path": (root_path + "C_eyelash_GEO"),
    },
    "mouth_shapes": {"name": "mouth_shapes", "path": (root_path + "C_headMouth_GEO"),},
    "brow_shapes": {"name": "brow_shapes", "path": (root_path + "C_brow_GEO")},
    "C_facial_hair_BLD": {
        "name": "C_facial_hair_BLD",
        "path": (beard_path + "C_facial_hair_GEO"),
    },
}

sb_path = ""
avatar_exp_list = {}
prop_exp_list = {}

# queries scene time and unit
SCENE_TIME = cmds.currentUnit(query=True, time=True)
SCENE_UNIT = cmds.currentUnit(query=True)
SCENE_FPS = mel.eval("float $fps = `currentTimeUnitToFPS`")


def face_uv_anim_keys():
    """
    saves face uv animation values
    """
    anim_control = None
    frame_rate = SCENE_FPS
    key_values = {"frames": [], "expressions": []}
    anim_controls = cmds.ls(BAKE_KW["FACE_ANIM_CONTROL"]) or cmds.ls(
        "*:*" + BAKE_KW["FACE_ANIM_CONTROL"]
    )
    anim_control = anim_controls[0]
    if not anim_controls or not len(anim_controls) == 1:
        raise RuntimeError("face animation control was not found")
    attr_path = "{}.{}".format(anim_control, BAKE_KW["FACE_ANIM_ATTR"])
    if not cmds.objExists(attr_path):
        raise RuntimeError("attribute {} not found ".format(attr_path))
    frame_value = cmds.keyframe(
        attr_path, query=True, timeChange=True, valueChange=True
    )
    if not frame_value:
        return {}
    for frame, value in zip(frame_value[::2], frame_value[1::2]):
        value = round(value)
        if frame != frame_value[0] and value == last_value:
            pass  # first frame or no value change
        else:
            key_values["frames"].append(frame)
            key_values["expressions"].append(BAKE_KW["TEXTURE_LOOKUP"][int(value)])
        last_value = value
    key_values["frame_rate"] = SCENE_FPS
    return key_values


def delete_unused(top_node_name):
    """
    deletes everything outise the top node
    """
    top_node = cmds.ls(top_node_name, dag=True)
    extra_nodes = cmds.ls("*", transforms=True)
    default_cameras = set(cmds.ls("front", "persp", "side", "top"))
    list_nodes = list(set(extra_nodes) - default_cameras)
    for x in list_nodes:
        if x not in top_node:
            cmds.delete(x)


def baker():
    """
    bake animation
    """
    import_references()
    blendshapes = cmds.ls(type="blendShape")
    startFrame = int(cmds.playbackOptions(query=True, min=True))
    endFrame = int(cmds.playbackOptions(query=True, max=True))
    bind_jnt = cmds.ls("*:*bind_GRP", "*:*geo_GRP", "*bind_GRP", "BAKE", dag=True)
    cmds.select(bind_jnt, blendshapes)
    cmds.bakeResults(t=(startFrame, endFrame), sm=True)


def add_key_at_first_frame():
    """
    add key at frame 1 on all blendshape noes & joints
    """
    blendshapes = cmds.ls(type="blendShape")
    cmds.currentTime(1)
    for node in blendshapes:
        cmds.setKeyframe(node)

    all_jnt_list = cmds.ls(type="joint")

    for single_jnt in all_jnt_list:
        for rot in BAKE_KW["attrR"]:
            cmds.setKeyframe(single_jnt, attribute=rot)

    try:
        for item in BAKE_KW["FACEBONE_EXCLUDE"]:
            all_jnt_list.remove(item)
    except ValueError:
        pass

    for single_jnt in all_jnt_list:
        for trans in BAKE_KW["attrT"]:
            cmds.setKeyframe(single_jnt, attribute=trans)


def add_scale_key_at_first_frame():
    """
    add scale key at first frame on root joint of prop
    """
    try:
        name_len = {}
        cmds.currentTime(1)
        jnt_list = cmds.listRelatives(
            cmds.ls("PROP"), allDescendents=True, type="joint", fullPath=True
        )
        for jnt in jnt_list:
            name_len[jnt] = len(jnt.split("|"))
        flip_jnt_list = dict((v, k) for k, v in name_len.items())
        top_jnt = flip_jnt_list.values()[0]
        for sc in BAKE_KW["attrS"]:
            cmds.setKeyframe(top_jnt, attribute=sc)
    except:
        print "Can not find top joint on prop", "=" * 80
        pass


def clean_scene():
    euler_filter()
    cmds.delete(all=True, staticChannels=True)
    cleanup_scene()


def facial_animation_dict():
    """
    for console rig
    return : facial & beard animation data
    """

    for key, value in shape_list.items():
        try:
            cmds.setKeyframe(value["name"], t=[1])
        except ValueError:
            print "No object maches name to : {}".format(key)
            pass

    # get facial animation data
    for key, value in shape_list.items():
        try:
            for attr in cmds.listAttr((value["name"] + ".weight"), multi=True):
                attr_name = value["name"] + "." + attr
                keyframe_val = cmds.keyframe(
                    value["name"], attribute=attr, q=True, tc=True, vc=True
                )
                if keyframe_val:
                    key_anim_temp[attr_name] = keyframe_val
        # pass if object is not exist
        except ValueError:
            print "No object matches name to ".format(value["name"])
            pass

    for key, frame_value in key_anim_temp.items():
        key_values = {"frames": [], "value": [], "path": ""}
        for frame, value in zip(frame_value[::2], frame_value[1::2]):
            value = round(value, 4)  # round to .xxxx instead to int
            key_values["frames"].append(frame)
            key_values["value"].append(value)
            key_values["path"] = shape_list[key.split(".")[0]]["path"]
            last_value = value
        key_anim_dict[key] = key_values

    # beard blendshape (copied from face_shapes)
    for key_k, value_k in key_anim_dict.items():
        if "face_shapes" in key_k:
            anim_values = {"frames": [], "value": [], "path": ""}
            anim_values["frames"] = key_anim_dict[key_k]["frames"]
            anim_values["value"] = key_anim_dict[key_k]["value"]
            anim_values["path"] = shape_list["C_facial_hair_BLD"]["path"]
            facial_hair_key = (
                shape_list["C_facial_hair_BLD"]["name"] + "." + key_k.split(".")[1]
            )
            facialhair_dict[facial_hair_key] = anim_values

    face_dict = dict(key_anim_dict, **facialhair_dict)

    return face_dict, shape_list


def show_joints():
    """
    shows bind joints
    """
    for j in cmds.ls(type="joint"):
        cmds.setAttr(j + ".drawStyle", 0)


def scale_compensate():
    """
    turn off segment scale compensate
    """
    jnt_list = cmds.ls(type="joint")
    for single_jnt in jnt_list:
        cmds.setAttr((single_jnt + ".segmentScaleCompensate"), 0)


def bake_is_done_message_maya():
    message = cmds.confirmDialog(
        title="BAKE ANIMATION",
        message="Bake is completed",
        button=["Ok"],
        defaultButton="Ok",
        cancelButton="Ok",
    )


def bake_is_done_message(file_path):
    msg = "msg %username% {}{}".format(file_path, "  :   baked! ")
    os.system(msg)


def current_workspace():
    wkspace = cmds.file(sceneName=True, query=True, shortName=True)
    name = "_anm_heavy" if "heavy" in wkspace else "_anm_default"
    project_name = wkspace.split(name)[0]
    scene_name = (
        project_name + "_anm_heavy"
        if "heavy" in name
        else project_name + "_anm_default"
    )
    return project_name, scene_name, name


def _maya_source_mel(script):
    maya_path_a = os.environ["MAYA_SCRIPT_PATH"].split(":")
    maya_path_b = os.environ["MAYA_SCRIPT_PATH"].split(";")
    for path in maya_path_a or maya_path_b:
        script_path = os.path.join(path, script)
        if os.path.exists(script_path):
            mel.eval('source "{}"'.format(script_path.replace("\\", "\\\\")))
            return True
    return False


def cleanup_scene(*cleanup_args):
    """
    optimize scene size
    """
    options = cleanup_args or BAKE_KW["OPTIMIZE_OPTION"]
    mel.eval(
        "OptimizeSceneOptions;if (`window -exists OptionBoxWindow`) deleteUI -window OptionBoxWindow;"
    )
    _maya_source_mel("cleanUpScene.mel")
    os.environ["MAYA_TESTING_CLEANUP"] = str(1)
    for option in options:
        mel.eval('scOpt_performOneCleanup( {{ "{}Option" }} );'.format(option))
    os.environ["MAYA_TESTING_CLEANUP"] = str(0)


def save_path():
    """
    opens a dialog box to select the path to save the files
    """
    save_path = cmds.fileDialog2(
        fileFilter="*.fbx",
        dialogStyle=2,
        fileMode=0,
        caption="Export",
        okCaption="Export",
    )[0]
    file_name = save_path.split("/")[-1]
    file_path = save_path.split(file_name)[0]

    return save_path, file_path


def get_path(save_path):
    """
    returns file path and file name
    """
    file_name = save_path.split("/")[-1]
    file_path = save_path.split(file_name)[0]

    return save_path, file_path


def import_references():
    """
    imports referenced files into scene
    """
    references = cmds.file(query=True, reference=True)
    for ref in references:
        cmds.file(ref, importReference=True)


def write_json(data, path):
    with open(path, "w") as json_file:
        dump(data, json_file, sort_keys=True)


def write_visibility_data(data, path):
    file_path = os.path.expanduser(path)
    with open(file_path, "w") as json_file:
        dump(data, json_file)
    with file(file_path, "r") as original:
        data = original.read()
    with file(file_path, "w") as modified:
        modified.write("global.visibilityData = " + data)


def euler_filter_mobile():
    """
    runs euler filter on bind joints
    """
    bind_jnt = cmds.ls("*:*:*bind_JNT", "*:*bind_JNT", dag=True)
    binds = cmds.select(bind_jnt)
    curves = cmds.selectKey(k=True, add=True)
    cmds.filterCurve(filter="euler")


def euler_filter():
    binds = cmds.select(cmds.ls("*bind_JNT", "*:*bind_JNT"))
    curves = cmds.selectKey(k=True, add=True)
    cmds.filterCurve(filter="euler")


def export_fbx(path, sel=None):
    """
    exports FBX animation in the selected location
    """
    fbx_plugin = cmds.pluginInfo("fbxmaya", query=True, loaded=True)
    if fbx_plugin == False:
        cmds.loadPlugin("fbxmaya")
    mel.eval("FBXExportUseSceneName -v true;")
    if sel == None:
        file_export = cmds.file(
            path, force=True, exportSelected=True, type="FBX export"
        )
    else:
        file_export = cmds.file(path, force=True, exportAll=True, type="FBX export")


def unlock_nodes(node_name):
    if cmds.objExists(node_name):
        cmds.lockNode(node_name, lock=False)
        cmds.delete(node_name)


def remove_namespace(name):
    avatar = cmds.ls(name)
    for x in avatar:
        avatar_namespace = x.split(":")[0]
        cmds.namespace(set=":")
        cmds.namespace(removeNamespace=avatar_namespace, mergeNamespaceWithRoot=True)


def clean_avatar():
    """
    Clean up nodes for console rig
    """
    for node in BAKE_KW["CLEAN_AVATAR_NODES"]:
        if cmds.objExists(node):
            cmds.delete(node)
    show_joints()


def remove_geo():
    """
    remove geometry for unity console
    """
    if cmds.objExists("geo_GRP"):
        cmds.delete("geo_GRP")


def clean_nodes_mobile():
    """
    cleans hierarchy leaving only the bind group
    """
    remove_namespace("*:*AVATAR")
    for x in BAKE_KW["UNLOCK_NODES"]:
        unlock_nodes(x)
    cmds.parent("*:*bind_GRP", "AVATAR")
    remove_namespace("*:*bind_GRP")
    cmds.delete("rig_GRP", "geo_GRP")
    cmds.delete(all=True, staticChannels=True)
    cleanup_scene()
    # delete_unused("AVATAR")
    cmds.select("C_spine0001_bind_JNT")
    cmds.select("|AVATAR", add=1)
    cmds.parent()
    cmds.delete("|AVATAR|bind_GRP")


def clean_nodes_console():
    remove_namespace("*:*AVATAR")

    for node in BAKE_KW["UNLOCK_NODES"]:
        if cmds.objExists(node):
            cmds.lockNode(node, lock=False)
            cmds.delete(node)


def sort_list(a_list):
    sort_a = sorted(a_list)
    sort_b = sort_a[-1:] + sort_a[:-1]
    return sort_b


def save_fbx_lens_console(file_path):
    """
    for console rig
    """
    dup_list = []
    prop_list = []
    cmds.select("AVATAR")
    export_fbx(file_path)

    if cmds.objExists("*:PROP"):
        prop_top_nodes = cmds.ls("*:*PROP", dag=0)
        similar_props = []
        p_list = []

        for prop in prop_top_nodes:
            prefix = "_".join((prop.split("_"))[:-1]) + "_rig"
            best_match = difflib.get_close_matches(
                prefix, prop_top_nodes, 99, cutoff=0.7
            )
            if len(best_match) > 1:
                p_list.append(best_match)

        dict_tuple = {tuple(item): index for index, item in enumerate(p_list)}
        dup_list = [list(itm) for itm in dict_tuple.keys()]
        set_dup = []
        for item_list in dup_list:
            for item in item_list:
                set_dup.append(item)

        prop_list = list(set(prop_top_nodes) - set(set_dup))

    if dup_list:
        for a_list in dup_list:
            sorted_d_list = sort_list(a_list)
            export_props(file_path, sorted_d_list, same_items="yes")

    if prop_list:
        sorted_p_list = sort_list(prop_list)
        export_props(file_path, sorted_p_list)


def export_props(file_path, props, same_items=None):
    alphabet = map(chr, range(65, 91))
    anum = 0
    for prop in props:
        remove_namespace(prop)
        if same_items == "yes":
            prop_name = (
                current_workspace()[1].replace("anm", "bke")
                + "_"
                + "_".join(prop.split("_")[:-1])
                + "_"
                + alphabet[anum]
                + ".fbx"
            )
        else:
            prop_name = (
                current_workspace()[1].replace("anm", "bke")
                + "_"
                + "_".join(prop.split("_")[:-1])
                + "_"
                + alphabet[0]
                + ".fbx"
            )
        anum += 1
        prop_path = os.path.join(os.path.dirname(file_path), prop_name)
        prop_exp_list[prop] = prop_path
        add_scale_key_at_first_frame()
        cmds.select("PROP")
        export_fbx(prop_path)
        cmds.delete("PROP")


def bake_scene_lens_console(export_file_path):
    """
    output : animation (fbx)
    """
    baker()
    clean_nodes_console()
    clean_avatar()
    clean_scene()
    add_key_at_first_frame()
    save_fbx_lens_console(export_file_path)
    bake_is_done_message(export_file_path)
    return True


def bake_scene_unity_console(export_file_path):
    """
    output : animation (fbx) , facial anim (json)
    """
    baker()
    clean_nodes_console()
    clean_avatar()
    clean_scene()
    add_key_at_first_frame()
    facial_animation = facial_animation_dict()[0]
    json_path = os.path.splitext(export_file_path)[0] + ".json"
    write_json(facial_animation, json_path)
    remove_geo()
    scale_compensate()
    cmds.select("AVATAR")
    export_fbx(export_file_path)
    bake_is_done_message(export_file_path)
    return True


def bake_scene_mobile(export_file_path, platform=None):
    """
    output : animation (fbx) , facial anim (json)
    """
    import_references()
    cmds.currentUnit(time=SCENE_TIME)
    cmds.currentUnit(linear=SCENE_UNIT)
    start_frame = int(cmds.playbackOptions(query=True, min=True))
    end_frame = int(cmds.playbackOptions(query=True, max=True))
    binds = "*:*:*bind_JNT", "*:*bind_JNT"
    face_shapes = "*:*C_head_polyMove", "C_head_polyMove"
    bind_jnt = cmds.ls(binds, face_shapes, dag=True)
    cmds.select(bind_jnt)
    cmds.bakeResults(t=(start_frame, end_frame), sm=True)
    euler_filter_mobile()
    show_joints()

    if platform == "playcanvas":
        export_path = get_path(export_file_path)
        write_json(face_uv_anim_keys(), (export_path[1] + "/faceAnim.json"))
    elif platform == "unity":
        json_path = os.path.splitext(export_file_path)[0] + ".json"
        write_json(face_uv_anim_keys(), json_path)

    clean_nodes_mobile()
    scale_compensate()
    cmds.select("AVATAR")
    export_fbx(export_file_path)
    bake_is_done_message(export_file_path)
    return True


def remove_files(*args):
    for _file in args:
        try:
            os.remove(_file)
            os.chmod(_file, 0o777)
        except:
            pass


def warn_message_window(msg):
    message = cmds.confirmDialog(
        title="Error",
        message=msg,
        button=["Ok"],
        defaultButton="Ok",
        cancelButton="Ok",
    )


def export_maya(path):
    cmds.file(
        path, force=True, type="mayaAscii", exportAll=True, preserveReferences=True
    )


def maya_main_window():
    main_window_ptr = omui.MQtUtil.mainWindow()
    return wrapInstance(long(main_window_ptr), QtWidgets.QWidget)


class SnapBakeWindow(QtWidgets.QDialog):
    def __init__(self, parent=maya_main_window()):
        super(SnapBakeWindow, self).__init__(parent)

        self.setWindowTitle("Snap Bake v1.1.0")
        self.setWindowFlags(self.windowFlags() ^ QtCore.Qt.WindowContextHelpButtonHint)

        self.create_widgets(self)
        self.create_connections()
        QtCore.QMetaObject.connectSlotsByName(self)

    def create_widgets(self, SBW):
        SBW.setObjectName("SBW")
        self.width = 262
        self.length = 189
        self.resize(self.width, self.length)
        self.setMinimumSize(self.width, self.length)
        self.setMaximumSize(self.width, self.length)
        self.ui = QtWidgets.QFrame()
        self.ui.setGeometry(20, 110, 231, 31)
        self.ui.setFrameShape(QtWidgets.QFrame.HLine)
        self.ui.setFrameShadow(QtWidgets.QFrame.Sunken)
        self.ui.setObjectName("ui")

        self.group_box = QtWidgets.QGroupBox(SBW)
        self.group_box.setGeometry(10, 20, 241, 91)
        self.group_box.setTitle("")
        self.group_box.setObjectName("group_box")

        self.label_a = QtWidgets.QLabel(self.group_box)
        self.label_a.setGeometry(-20, 10, 71, 21)
        self.label_a.setAlignment(QtCore.Qt.AlignRight | QtCore.Qt.AlignVCenter)
        self.label_a.setObjectName("label_a")
        self.label_a.setText("Engine")

        self.path_box = QtWidgets.QLineEdit(self.group_box)
        self.path_box.setGeometry(60, 50, 141, 23)
        self.path_box.setObjectName("path_box")

        self.path_btn = QtWidgets.QPushButton(self.group_box)
        self.path_btn.setGeometry(200, 49, 31, 25)
        self.path_btn.setObjectName("path_btn")
        self.path_btn.setText("Path")

        self.label_b = QtWidgets.QLabel(self.group_box)
        self.label_b.setGeometry(0, 50, 51, 21)
        self.label_b.setAlignment(QtCore.Qt.AlignRight | QtCore.Qt.AlignVCenter)
        self.label_b.setObjectName("label_b")
        self.label_b.setText("Save to")

        self.engine_menu = QtWidgets.QComboBox(self.group_box)
        self.engine_menu.setGeometry(60, 10, 171, 25)
        self.engine_menu.setObjectName("engine_menu")
        self.engine_menu.addItem("")
        self.engine_menu.addItem("")
        self.engine_menu.addItem("")
        self.engine_menu.addItem("")
        self.engine_menu.setItemText(0, "Unity - Console")
        self.engine_menu.setItemText(1, "Unity - Mobile")
        self.engine_menu.setItemText(2, "PlayCanvas - Console")
        self.engine_menu.setItemText(3, "PlayCanvas - Mobile")

        self.export_btn = QtWidgets.QPushButton(SBW)
        self.export_btn.setGeometry(QtCore.QRect(20, 140, 221, 31))
        self.export_btn.setObjectName("export_btn")
        self.export_btn.setText("Export")

    def create_connections(self):
        self.path_btn.clicked.connect(self.save_path)
        self.export_btn.clicked.connect(self.export_exc)

    def save_path(self):
        """
        opens a dialog box to select the path to save the files
        """
        save_path = cmds.fileDialog2(
            fileFilter="*.fbx",
            dialogStyle=2,
            fileMode=0,
            caption="Set path to save files",
            okCaption="OK",
        )[0]
        self.path_box.setText(save_path)

    def export_exc(self):
        engine_sel = self.engine_menu.currentText()

        try:
            path_sel = self.path_box.text()
        except ValueError:
            warn_message_window("Select folder to save files")

        if engine_sel == "Unity - Console":
            bake_scene_unity_console(path_sel)
        elif engine_sel == "Unity - Mobile":
            bake_scene_mobile(path_sel, platform="unity")
        elif engine_sel == "PlayCanvas - Console":
            bake_scene_lens_console(path_sel)
        elif engine_sel == "PlayCanvas - Mobile":
            bake_scene_mobile(path_sel, platform="unity")
        else:
            print "Select engine type"


if __name__ == "__main__":
    try:
        cmp_ui.close()
        cmp_ui.deleteLater()
    except:
        pass

    cmp_ui = SnapBakeWindow()
    cmp_ui.show()
