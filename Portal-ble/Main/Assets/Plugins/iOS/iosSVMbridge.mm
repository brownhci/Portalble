//
//  iosSVMbridge.mm
//  Unity-iPhone
//
//  Created by HCI Lab1 on 2/6/20.
//

#import <opencv2/opencv.hpp>
#import <opencv2/imgcodecs/ios.h>
#import <Foundation/Foundation.h>
#include "iosSVM.hpp"

extern "C" {
    iosSVM svm_model;
    
    void LoadSVM(string msg)
    {
        svm_model.ios_load_model(msg);
    }
    
    int PredictSVM(int mat_n, float sent_data[])
    {
        return svm_model.ios_predict(mat_n, sent_data);
    }
}
