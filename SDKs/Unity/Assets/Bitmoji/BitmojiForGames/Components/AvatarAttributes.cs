using System.Collections.Generic;
using UnityEngine;

namespace Bitmoji.BitmojiForGames.Components
{
    public class AvatarAttributes : MonoBehaviour
    {
        public Assets.CharacterGender Gender { get; internal set; }
        public string AnimationBodyType { get; internal set; }
    }
}
