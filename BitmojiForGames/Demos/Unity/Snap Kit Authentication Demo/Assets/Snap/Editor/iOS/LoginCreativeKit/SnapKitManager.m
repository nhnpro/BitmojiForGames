#import "SnapKitManager.h"

@interface SnapKitManager()
@property(strong) SCSDKSnapAPI *snapApi;
@end


@implementation SnapKitManager

+ (SnapKitManager*)sharedManager
{
	static dispatch_once_t once;
	static id _instance;
	dispatch_once( &once,
	^{
		_instance = [[self alloc] init];
		[SCSDKLoginClient addLoginStatusObserver:_instance];
		[[NSNotificationCenter defaultCenter] addObserver:_instance selector:@selector(onApplicationOpenUrl:) name:@"kUnityOnOpenURL" object:nil];
	});
	
	return _instance;
}

+ (NSObject*)objectFromJsonString:(NSString*)json
{
	NSError *error = nil;
	NSData *data = [json dataUsingEncoding:NSUTF8StringEncoding];
	NSObject *object = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];
	
	if( error )
		NSLog( @"failed to deserialize JSON: %@ with error: %@", json, error );
	
	return object;
}

+ (NSString*)jsonStringFromObject:(NSObject*)object
{
	if( ![NSJSONSerialization isValidJSONObject:object] || !object )
	{
		NSLog( @"the object that is being serialized [%@] is not a valid JSON object", object );
		return @"{}";
	}
	
	NSError *error = nil;
	NSData *jsonData = [NSJSONSerialization dataWithJSONObject:object options:0 error:&error];
	if( jsonData && !error )
		return [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
	else
		NSLog( @"jsonData was null, error: %@", [error localizedDescription] );
	
	return @"{}";
}

- (void)onApplicationOpenUrl:(NSNotification*)note
{
	BOOL handled = [SCSDKLoginClient application:UIApplication.sharedApplication openURL:note.userInfo[@"url"] options:note.userInfo];
	NSLog(@"openURL handled by Snap: %@", handled ? @"yes" : @"no");
}

- (void)fetchUserDataWithQuery:(NSString *)query variables:(NSDictionary<NSString *, id> *)variables
{
	[SCSDKLoginClient fetchUserDataWithQuery:query variables:variables success:^(NSDictionary* resources) {
		NSString* json = [SnapKitManager jsonStringFromObject:resources];
		UnitySendMessage("SnapKitManager", "FetchUserDataSucceeded", json.UTF8String);
	} failure:^(NSError * _Nullable error, BOOL isUserLoggedOut) {
		UnitySendMessage("SnapKitManager", "FetchUserDataFailed", error != nil ? error.localizedDescription.UTF8String : "");
	}];
}

///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark CreativeKit

- (void)send:(NSDictionary*)dict
{
	self.snapApi = [SCSDKSnapAPI new];
	NSNumber* shareKind = (NSNumber*)dict[@"shareKind"];
	
	NSURL* shareFileUrl = nil;
	if (shareKind.intValue != 2 /* NoSnap */)
	{
		NSString* file = [self assureFilePath:dict[@"file"]];
		if (file == nil)
		{
			UnitySendMessage("SnapKitManager", "OnSendFinished", "invalid file path. files must either be a valid absolute path or a filename for a file in StreamingAssets");
			return;
		}
		shareFileUrl = [NSURL fileURLWithPath:file];
	}
	
	NSObject<SCSDKSnapContent>* shareContent = nil;
	switch (shareKind.intValue)
	{
		case 0: // photo
		{
			SCSDKSnapPhoto* photo = [[SCSDKSnapPhoto alloc] initWithImageUrl:shareFileUrl];
			shareContent = [[SCSDKPhotoSnapContent alloc] initWithSnapPhoto:photo];
			break;
		}
		case 1: // video
		{
			SCSDKSnapVideo* video = [[SCSDKSnapVideo alloc] initWithVideoUrl:shareFileUrl];
			shareContent = [[SCSDKVideoSnapContent alloc] initWithSnapVideo:video];
			break;
		}
		case 2: // no-snap
		{
			shareContent = [[SCSDKNoSnapContent alloc] init];
			break;
		}
	}
	
	// handle the optional sticker
	BOOL hasSticker = ((NSNumber*)dict[@"hasSticker"]).boolValue;
	if (hasSticker)
	{
		NSDictionary* stickerDict = dict[@"Sticker"];
		NSString* stickerFile = [self assureFilePath:stickerDict[@"file"]];
		if (stickerFile == nil)
		{
			UnitySendMessage("SnapKitManager", "OnSendFinished", "invalid sticker file path. files must either be a valid absolute path or a filename for a file in StreamingAssets");
			return;
		}
		
		SCSDKSnapSticker *sticker = [[SCSDKSnapSticker alloc] initWithStickerUrl:[NSURL fileURLWithPath:stickerFile] isAnimated:NO];
		
		float value = ((NSNumber*)stickerDict[@"PosX"]).floatValue;
		if (value != 0)
			sticker.posX = value;
		
		value = ((NSNumber*)stickerDict[@"PosY"]).floatValue;
		if (value != 0)
			sticker.posY = value;
		
		value = ((NSNumber*)stickerDict[@"Height"]).floatValue;
		if (value != 0)
			sticker.height = value;

		value = ((NSNumber*)stickerDict[@"Width"]).floatValue;
		if (value != 0)
			sticker.width = value;

		sticker.rotation = ((NSNumber*)stickerDict[@"RotationDegreesClockwise"]).floatValue;
		shareContent.sticker = sticker;
	}
	
	NSString* attachmentUrl = dict[@"AttachmentUrl"];
	if (attachmentUrl != nil && attachmentUrl.length > 0)
		shareContent.attachmentUrl = attachmentUrl;
	
	NSString* captionText = dict[@"CaptionText"];
	if (captionText != nil && captionText.length > 0)
		shareContent.caption = captionText;
	
	// finally, send the share content
	[self.snapApi startSendingContent:shareContent completionHandler:^(NSError *error)
	{
		self.snapApi = nil;
		if (error)
			UnitySendMessage("SnapKitManager", "OnSendFinished", error.localizedDescription.UTF8String);
		else
			UnitySendMessage("SnapKitManager", "OnSendFinished", "");
    }];
}

- (NSString*)assureFilePath:(NSString*)file
{
	if ([file hasPrefix:@"/"])
	{
		if ([NSFileManager.defaultManager fileExistsAtPath:file])
			return file;
		return nil;
	}
	
	// check StreamingAssets folder
	NSString* basePath = [NSBundle.mainBundle.bundlePath stringByAppendingPathComponent:@"Data/Raw"];
	file = [basePath stringByAppendingPathComponent:file];
	
	if ([NSFileManager.defaultManager fileExistsAtPath:file])
		return file;
	return nil;
}

///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark SCSDKLoginStatusObserver

- (void)scsdkLoginLinkDidStart
{
	UnitySendMessage("SnapKitManager", "LoginLinkDidStart", "");
}

- (void)scsdkLoginLinkDidSucceed
{
	UnitySendMessage("SnapKitManager", "LoginLinkDidSucceed", "");
}

- (void)scsdkLoginLinkDidFail
{
	UnitySendMessage("SnapKitManager", "LoginLinkDidFail", "");
}

- (void)scsdkLoginDidUnlink
{
	UnitySendMessage("SnapKitManager", "LoginDidUnlink", "");
}

@end
