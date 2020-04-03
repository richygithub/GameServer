// ProtoPlugin.cpp : 此文件包含 "main" 函数。程序执行将在此处开始并结束。
//

#include <iostream>
#include <string>
#include <fstream>
#include "google\protobuf\compiler\plugin.pb.h"
#include "google\protobuf\compiler\plugin.h"
#include <google/protobuf/compiler/code_generator.h> 
#include <google/protobuf/compiler/plugin.h> 
#include <google/protobuf/descriptor.h>
#include <google/protobuf/io/coded_stream.h> 
#include <google/protobuf/io/zero_copy_stream.h>
using namespace std;

//using namespace google.protobuf;
using namespace google::protobuf;
using namespace google::protobuf::compiler;
class PodCodeGenerator : public CodeGenerator
{
public:
    virtual ~PodCodeGenerator() {};
    virtual bool Generate(const ::google::protobuf::FileDescriptor* file_, const string& parameter, GeneratorContext* context, string* error) const
    {
        // 根据FileDescriptor，产生输出文件的信息
        //cout << "OK!!\n" << file_->name() << endl;
         
        const FileDescriptor& file = *file_;
        
        ofstream fout("test.txt");
        fout << "ok" << endl;
        return true;
    }
};
int main(int argc,char*argv[])
{
    /*
    int n = 1;
    char file[] = "address.proto";
    char* param[] = { file };
    */

    PodCodeGenerator generator;
    return google::protobuf::compiler::PluginMain(argc, argv, &generator);
    //int len=sizeof(argv) / sizeof(char*);
    for (int idx = 0; idx < argc; idx++) {
        cout << idx << ":" << argv[idx]<<endl;
    }
    /*
    string str;
    cout << "getStdIn" << endl;
    while (getline(cin, str)) {
        cout << str << endl;
    }
    */
    ifstream  afile;
    afile.open("address.proto", ios::out | ios::in);
    auto req = new google::protobuf::compiler::CodeGeneratorRequest();
    while (!afile.eof()) {
        string line;
        //afile.getline(;

    }

    bool ret = req->ParseFromIstream(&afile);

    std::cout << "Hello World!\n"<<ret;
}

// 运行程序: Ctrl + F5 或调试 >“开始执行(不调试)”菜单
// 调试程序: F5 或调试 >“开始调试”菜单

// 入门使用技巧: 
//   1. 使用解决方案资源管理器窗口添加/管理文件
//   2. 使用团队资源管理器窗口连接到源代码管理
//   3. 使用输出窗口查看生成输出和其他消息
//   4. 使用错误列表窗口查看错误
//   5. 转到“项目”>“添加新项”以创建新的代码文件，或转到“项目”>“添加现有项”以将现有代码文件添加到项目
//   6. 将来，若要再次打开此项目，请转到“文件”>“打开”>“项目”并选择 .sln 文件
