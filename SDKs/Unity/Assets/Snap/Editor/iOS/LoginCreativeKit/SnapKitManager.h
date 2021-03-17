#import <Foundation/Foundation.h>
#import <SCSDKLoginKit/SCSDKLoginKit.h>
#import <SCSDKCreativeKit/SCSDKCreativeKit.h>

#ifdef __cplusplus
extern "C" {
#endif
	void UnitySendMessage(const char* obj, const char* method, const char* msg);
#ifdef __cplusplus
}
#endif

@interface SnapKitManager : NSObject<SCSDKLoginStatusObserver>

+ (SnapKitManager*)sharedManager;
+ (NSObject*)objectFromJsonString:(NSString*)json;
+ (NSString*)jsonStringFromObject:(NSObject*)object;

- (void)send:(NSDictionary*)dict;

@end
