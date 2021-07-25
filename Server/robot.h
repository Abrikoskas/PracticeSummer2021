#include "snap.h"
#include <mosquitto.h>

using namespace std;


void on_message(struct mosquitto* mosq, void* obj, const struct mosquitto_message* msg) { //printing message if it's publish in mosquitto
	printf("New message with topic %s: %s\n", msg->topic, (char*)msg->payload);
}

class Robot {
	private: 
		struct mosquitto* mosq;
		int rc;
		int id = 12;
		

		void publish(const char* message, const char* topic) { //publish in mosquitto
			mosquitto_publish(mosq, NULL, topic, strlen(message), message, 0, false); //Publish message to a topic "test/"
			mosquitto_message_callback_set(mosq, on_message);
		}

	public: 
		bool isOn = false;

		Robot() {
			mosquitto_lib_init();
			mosq = mosquitto_new("Server", true, &id);
			mosquitto_message_callback_set(mosq, on_message);
			rc = mosquitto_connect(mosq, "25.44.0.56", 1883, 60);

			if (rc != 0) {
				std::cout << "Client couldn't connect to broker!" << std::endl;
				mosquitto_destroy(mosq);
				return;
			}
			else {
				std::cout << "Client is now connected to the broker" << std::endl;
			}
			/*mosquitto_loop_start(mosq);*/
		}

		void on() { //subscribing on topic and turns robot online
			if (isOn == false) {
				const char* topic = "test/";
				mosquitto_subscribe(mosq, NULL, topic, 0); //Subscribing to a topic "test/"
				mosquitto_message_callback_set(mosq, on_message);
				const char* message = "The robot is online now! You can send your commands to it!";
				mosquitto_loop_start(mosq);
				publish(message, topic);
				isOn = true;
			}
		}

		void takeSnap() { // taking screenshot
			mosquitto_loop_start(mosq);
			publish("Snap", "test/camera/");
			cout << "Tooked screenshot" << endl;
		} 
		
		void findPivo(Snap snap) { //checking if it's Pivo on the picture
			const char* topic = "test/";
			const char* message;
			if (snap.compare()) {
				message = "I've found the pivo";
			}
			else {
				message = "I can't find pivo :(";
			}

			mosquitto_loop_start(mosq);
			publish(message, topic);
		}

		bool notOffline() { //check if robot online/offline
			return isOn;
		}

		void moveRight() { //sending to Robot command to move his cmaera right
			const char* topic = "test/";
			mosquitto_loop_start(mosq);
			
			publish("Right", topic);
		}

		void moveLeft() { //sending to Robot command to move his cmaera left
			const char* topic = "test/";
			mosquitto_loop_start(mosq);
			publish("Left", topic);
		}

		void moveForward() { //sending to Robot command to move forward
			const char* topic = "test/";
			mosquitto_loop_start(mosq);
			publish("Forward", topic);
		}

		void moveBackward() { //sending to Robot command to move backward
			const char* topic = "test/";
			mosquitto_loop_start(mosq);
			publish("Backward", topic);
		} 

		void off() { // turns robot offline
			const char* topic = "test/";
			const char* message = "If you want to recconect to the robot you need to reload your programm!";
			mosquitto_loop_start(mosq);
			publish("The robot turns off...", topic);
			publish(message, topic);
			mosquitto_loop_stop(mosq, true);
			isOn = false;
		}
};



