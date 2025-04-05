#include <laser.h>
#include <pressao.h>
#include <vector>

class Data {
    private:
        Laser laser;
        Pressao pressao;
    public:
        Data() {}
        void start() {
            laser.iniciar();
            pressao.iniciar();
        }
        std::vector<double> getDados() {
            std::vector<double> dados;
            dados.push_back(laser.getAltura());
            dados.push_back(pressao.getAltura());
            return dados;
        }
};