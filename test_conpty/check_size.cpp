#include <windows.h>
#include <iostream>

int main() {
    std::cout << "STARTUPINFO: " << sizeof(STARTUPINFOA) << std::endl;
    std::cout << "STARTUPINFOEX: " << sizeof(STARTUPINFOEXA) << std::endl;
    return 0;
}
