//
//  SVM.hpp
//  Unity-iPhone
//
//  Created by HCI Lab1 on 2/6/20.
//

#ifndef SVM_hpp
#define SVM_hpp

using namespace cv::ml;
using namespace std;

#include <stdio.h>

class iosSVM {
public:
    cv::Ptr<SVM> svm_model = SVM::create();
    // SVM svm_model = SVM::create();
    void ios_load_model(const string& filepath);
    int ios_predict(int mat_n, float sent_data[]);
};
#endif /* SVM_hpp */
