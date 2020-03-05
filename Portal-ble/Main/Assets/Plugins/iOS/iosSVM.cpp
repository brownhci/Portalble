//
//  SVM.cpp
//  Unity-iPhone
//
//  Created by HCI Lab1 on 2/6/20.
//
#include <opencv2/opencv.hpp>
#include "iosSVM.hpp"
#include <iostream>
#include <fstream>

using namespace cv::ml;
using namespace std;

extern "C" {
    // cv::Ptr<cv::ml::SVM> svm_model = cv::ml::SVM::create();

    void iosSVM::ios_load_model(const string& filepath) {
        svm_model = svm_model->load(filepath);
        ifstream myfile(filepath);
        string line;
        if (myfile.is_open())
        {
            getline (myfile,line);
            cout << line << '\n';
        }
        else cout << "Unable to open file";
        std::cout << "Loading SVM from:";
        std::cout << filepath;
    }

    int iosSVM::ios_predict(int mat_n, float sent_data[]) {
        cv::Mat cur_data_mat = cv::Mat(1, mat_n, CV_32F, sent_data);
        //std::memcpy(cur_data_mat.data, sent_data, 1*mat_n*sizeof(float));
        cv::Mat result = cv::Mat::ones(1,1,CV_32S);
//        std::cout << "sent data: ";
//        std::cout << sent_data[0];
//        std::cout << ", ";
//        std::cout << sent_data[1];
//        std::cout << " cur_data_mat: ";
//        std::cout << cur_data_mat << endl;
        int p = svm_model->predict(cur_data_mat);
//        std::cout << "predicted value";
//        std::cout << p;
        return p;
    }
}
