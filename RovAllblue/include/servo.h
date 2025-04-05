#include <ESP32Servo.h>

class ServoController {
    private:
      int pin;
      ESP32PWM pwm;
      int angle; // Store the current angle (in degrees)
  
    public:
      // Constructor that initializes the pin and frequency
      ServoController(int pin, int freq) {
          this->pin = pin;
          this->angle = 0; // Initially, the servo is at 0 degrees
          ESP32PWM::allocateTimer(0); // Allocate the timer
          pwm.attachPin(pin, freq, 10); // Attach PWM to the pin
      }
  
      // Method to write the servo angle (in the range 0° to 180°)
      void writeAngle(int newAngle) {
          // Clamp the value to the range [0, 180]
          if (newAngle < 0) newAngle = 0;
          if (newAngle > 180) newAngle = 180;
  
          // Convert the angle to pulse width (in microseconds)
          pwm.write(newAngle); // Send the PWM signal with the corresponding pulse width
          angle = newAngle; // Update the angle value
      }

      // Method to return the current angle
      int getAngle() {
          return angle; // Return the current stored angle
      }
  };