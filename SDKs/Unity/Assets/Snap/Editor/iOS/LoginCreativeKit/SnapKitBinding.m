#import "SnapKitManager.h"

#define GetStringParam( _x_ ) ( _x_ != NULL ) ? [NSString stringWithUTF8String:_x_] : [NSString stringWithUTF8String:""]
#define MakeStringCopy( _x_ ) ( _x_ != NULL && [_x_ isKindOfClass:[NSString class]] ) ? strdup( [_x_ UTF8String] ) : NULL

void _snapKitLogin() {
	[SnapKitManager sharedManager];
	
	UIViewController* vc = UIApplication.sharedApplication.keyWindow.rootViewController;
	[SCSDKLoginClient loginFromViewController:vc
								   completion:^(BOOL success, NSError * _Nullable error)
	{
		NSLog(@"SnapKit login completed. success: %d, error: %@", success, error);
		UnitySendMessage("SnapKitManager", "LoginCompleted", error != nil ? error.localizedDescription.UTF8String : "");
	}];
}

BOOL _snapKitIsLoggedIn() {
	return SCSDKLoginClient.isUserLoggedIn;
}

void _snapKitUnlinkAllSessions() {
	[SCSDKLoginClient clearToken];
}

const char* _snapKitGetAccessToken() {
	return MakeStringCopy([SCSDKLoginClient getAccessToken]);
}

BOOL _snapKitHasAccessToScope(const char* scope) {
	return [SCSDKLoginClient hasAccessToScope:GetStringParam(scope)];
}


void _snapKitFetchUserDataWithQuery(const char* query, const char* variables) {
	NSDictionary<NSString*,id>* dict = (NSDictionary*)[SnapKitManager objectFromJsonString:GetStringParam(variables)];
	
	[SCSDKLoginClient fetchUserDataWithQuery:GetStringParam(query) variables:dict success:^(NSDictionary* resources) {
		UnitySendMessage("SnapKitManager", "FetchUserDataSucceeded", [SnapKitManager jsonStringFromObject:resources].UTF8String);
	} failure:^(NSError * _Nullable error, BOOL isUserLoggedOut) {
		UnitySendMessage("SnapKitManager", "FetchUserDataFailed", error != nil ? error.localizedDescription.UTF8String : "");
	}];
}

void _snapKitVerify(const char* phone, const char* region) {
	[SCSDKVerifyClient verifyFromViewController:UIApplication.sharedApplication.keyWindow.rootViewController
										  phone:GetStringParam(phone)
										 region:GetStringParam(region)
									 completion:^(NSString * _Nullable phoneId, NSString * _Nullable verifyId, NSError * _Nullable error) {
		NSLog(@"verify done. error: %@, phoneId: %@, verifyId: %@", error, verifyId, phoneId);
		if (error) {
			UnitySendMessage("SnapKitManager", "VerifyFailed", error != nil ? error.localizedDescription.UTF8String : "");
		} else {
			NSDictionary* dict = @{
				@"phoneId": phoneId,
				@"verifyId": verifyId
			};
			UnitySendMessage("SnapKitManager", "VerifyCompleted", [SnapKitManager jsonStringFromObject:dict].UTF8String);
		}
	}];
}

void _creativeKitShare(const char* shareContent) {
	NSDictionary<NSString*,id>* dict = (NSDictionary*)[SnapKitManager objectFromJsonString:GetStringParam(shareContent)];
	[SnapKitManager.sharedManager send:dict];
}
