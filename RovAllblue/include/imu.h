#ifndef MPU6050_ZANGLE_H
#define MPU6050_ZANGLE_H

#include <Wire.h>
#include <MPU6050_light.h>

class imu {
public:
    
    imu();

    bool begin();

    float getZAngle();


private:
    MPU6050 mpu;          
    unsigned long timer;   
    bool isInitialized;    
};

#endif