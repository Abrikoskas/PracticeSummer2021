#define _CRT_SECURE_NO_WARNINGS

#include <iostream>
#include <stdio.h>
#include <stdlib.h>
#include <stdint.h>
#include <string>
#include <cmath>
#include "robot.h"
#include <conio.h>




int main() {
	
	Robot Valle;

	Snap picture = Snap("C:/Users/wodge/pivoDaleko.bmp");

	int input; 

	cout << "Press g to turn on Robot" << endl;
	input = _getch();

	if (input == 103) {
		Valle.on(); //103 - button g
		cout << "Commands to use Robot:" << endl << "press SPACE to take screenshot" << endl << "press ESC to turn off the robot" << endl << "use ARROWS to move the robot" << endl;
		cout << "use ENTER to check if it's pivo on the image." << endl;
	} else {
		cout << "User decided to not use the robot" << endl;
	}

	while (Valle.notOffline()) {
		
		input = _getch();

		switch (input) {
		
		case 72: Valle.moveForward(); break; //72 - code strelki vverx
		
		case 80: Valle.moveBackward(); break; //80 - code strelki vniz
		
		case 75: Valle.moveLeft(); break; //75 - code strelki vlevo
		
		case 77: Valle.moveRight(); break; //77 - code strelki vpravo
		
		case 32: Valle.takeSnap();  break; // 32 - code probela
		
		case 13: Valle.findPivo(picture); break; // 13 - code Enter
		
		case 27: Valle.off(); break; // 27 - code Escape
		}

	}

	return 0;
}