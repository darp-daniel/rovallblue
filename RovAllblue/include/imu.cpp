#include <imu.h>

imu::imu() : mpu(Wire), isInitialized(false) {}

bool imu::begin() {
    Wire.begin();
    byte status = mpu.begin();
    
    if (status != 0) {
        Serial.println("MPU6050 falhou");
        return false;
    }

    Serial.println("Calibrando");
    delay(1000);
    mpu.calcOffsets();  // Calibrate gyro/accel
    mpu.setFilterGyroCoef(1);
    isInitialized = true;
    Serial.println("MPU6050 pronto!");
    return true;
}

float imu::getZAngle() {
    if (isInitialized) {
        mpu.update();
        return mpu.getAngleZ();  // Return Z-axis angle in degrees
    };
    return 0.0;  // Default if sensor isn't initialized
}