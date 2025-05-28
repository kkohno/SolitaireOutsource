//
//  Picker.mm
//  Unity-iPhone
//
//  Created by thedoritos on 11/19/16.
//
//

#import "Picker.h"

#pragma mark Config

const char* CALLBACK_OBJECT = "Unimgpicker";
const char* CALLBACK_METHOD = "OnComplete";
const char* CALLBACK_METHOD_FAILURE = "OnFailure";

const char* MESSAGE_FAILED_PICK = "Failed to pick the image";
const char* MESSAGE_FAILED_FIND = "Failed to find the image";
const char* MESSAGE_FAILED_COPY = "Failed to copy the image";

#pragma mark Picker

@implementation Picker

+ (instancetype)sharedInstance {
    static Picker *instance;
    static dispatch_once_t token;
    dispatch_once(&token, ^{
        instance = [[Picker alloc] init];
    });
    return instance;
}

- (void)show:(NSString *)title outputFileName:(NSString *)name maxSize:(NSInteger)maxSize {
    if (self.pickerController != nil) {
        UnitySendMessage(CALLBACK_OBJECT, CALLBACK_METHOD_FAILURE, MESSAGE_FAILED_PICK);
        return;
    }
    
    self.maxSize = maxSize;
    
    self.pickerController = [[UIImagePickerController alloc] init];
    self.pickerController.delegate = self;
    
    self.pickerController.allowsEditing = NO;
    self.pickerController.sourceType = UIImagePickerControllerSourceTypePhotoLibrary;
    
    UIViewController *unityController = UnityGetGLViewController();
    [unityController presentViewController:self.pickerController animated:YES completion:^{
        self.outputFileName = name;
    }];
}



#pragma mark UIImagePickerControllerDelegate

+ (UIImage *)compressImage:(UIImage *)image maxSize:(NSInteger)maxSize{
    float actualHeight = image.size.height;
    float actualWidth = image.size.width;
    float maxHeight = maxSize;
    float maxWidth = maxSize;
    float imgRatio = actualWidth/actualHeight;
    float maxRatio = maxWidth/maxHeight;
    //float compressionQuality = 1;//50 percent compression
    
    if (actualHeight > maxHeight || actualWidth > maxWidth) {
        if(imgRatio < maxRatio){
            //adjust width according to maxHeight
            imgRatio = maxHeight / actualHeight;
            actualWidth = imgRatio * actualWidth;
            actualHeight = maxHeight;
        }
        else if(imgRatio > maxRatio){
            //adjust height according to maxWidth
            imgRatio = maxWidth / actualWidth;
            actualHeight = imgRatio * actualHeight;
            actualWidth = maxWidth;
        }else{
            actualHeight = maxHeight;
            actualWidth = maxWidth;
        }
    }
    
    CGRect rect = CGRectMake(0.0, 0.0, actualWidth, actualHeight);
    UIGraphicsBeginImageContext(rect.size);
    [image drawInRect:rect];
    UIImage *img = UIGraphicsGetImageFromCurrentImageContext();
    //NSData *imageData = UIImageJPEGRepresentation(img, compressionQuality);
    NSData *imageData = UIImagePNGRepresentation(img);

    UIGraphicsEndImageContext();
    
    return [UIImage imageWithData:imageData];
}

- (void)imagePickerController:(UIImagePickerController *)picker didFinishPickingMediaWithInfo:(NSDictionary<NSString *,id> *)info {
    UIImage *image = info[UIImagePickerControllerOriginalImage];
    if (image == nil) {
        UnitySendMessage(CALLBACK_OBJECT, CALLBACK_METHOD_FAILURE, MESSAGE_FAILED_FIND);
        [self dismissPicker];
        return;
    }
    
    image = [Picker compressImage:image maxSize:self.maxSize];
    
    NSArray *paths = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES);
    if (paths.count == 0) {
        UnitySendMessage(CALLBACK_OBJECT, CALLBACK_METHOD_FAILURE, MESSAGE_FAILED_COPY);
        [self dismissPicker];
        return;
    }
    
    NSString *imageName = self.outputFileName;
    if ([imageName hasSuffix:@".png"] == NO) {
        imageName = [imageName stringByAppendingString:@".png"];
    }
    
    NSString *imageSavePath = [(NSString *)[paths objectAtIndex:0] stringByAppendingPathComponent:imageName];
    NSData *png = UIImagePNGRepresentation(image);
    if (png == nil) {
        UnitySendMessage(CALLBACK_OBJECT, CALLBACK_METHOD_FAILURE, MESSAGE_FAILED_COPY);
        [self dismissPicker];
        return;
    }
    
    BOOL success = [png writeToFile:imageSavePath atomically:YES];
    if (success == NO) {
        UnitySendMessage(CALLBACK_OBJECT, CALLBACK_METHOD_FAILURE, MESSAGE_FAILED_COPY);
        [self dismissPicker];
        return;
    }
    
    UnitySendMessage(CALLBACK_OBJECT, CALLBACK_METHOD, [imageSavePath UTF8String]);
    
    [self dismissPicker];
}

- (void)imagePickerControllerDidCancel:(UIImagePickerController *)picker {
    UnitySendMessage(CALLBACK_OBJECT, CALLBACK_METHOD_FAILURE, MESSAGE_FAILED_PICK);
    
    [self dismissPicker];
}

- (void)dismissPicker
{
    self.outputFileName = nil;
    
    if (self.pickerController != nil) {
        [self.pickerController dismissViewControllerAnimated:YES completion:^{
            self.pickerController = nil;
        }];
    }
}

@end

#pragma mark Unity Plugin

extern "C" {
    void Unimgpicker_show(const char* outputFileName, int maxSize) {
        Picker *picker = [Picker sharedInstance];
        [picker show:[NSString stringWithUTF8String:""] outputFileName:[NSString stringWithUTF8String:outputFileName] maxSize:(NSInteger)maxSize];
    }
}
