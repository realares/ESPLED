#include <BitBool.h>             
#include <Math.h>

#include <WiFiUdp.h>
#include <WiFiServer.h>
#include <WiFiClientSecure.h>
#include <WiFiClient.h>
#include <ESP8266WiFi.h>
#include <Adafruit_NeoPixel.h>
#include <WiFiManager.h>
#include <DNSServer.h>            //Local DNS Server used for redirecting all requests to the configuration portal
#include <ESP8266WebServer.h>     //Local WebServer used to serve the configuration portal
#include <WiFiManager.h>          //https://github.com/tzapu/WiFiManager WiFi Configuration Magic

#include <Ticker.h> //for LED status

#define BLUE_LED_PIN 2
#define LEDPIN D4             // Neopixel Pin ( hier wurde der digitalpin f�r die Pixel angeschlossen! )
#define NUM_LEDS 60
#define BRIGHTNESS 255        // Helligkeit hardcoded
#define MAX_UDP_PACKET_SIZE 1024
#define UDP_PORT 2390


//Gamma-Table
byte neopix_gamma[] = {
  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  1,  1,  1,  1,
  1,  1,  1,  1,  1,  1,  1,  1,  1,  2,  2,  2,  2,  2,  2,  2,
  2,  3,  3,  3,  3,  3,  3,  3,  4,  4,  4,  4,  4,  5,  5,  5,
  5,  6,  6,  6,  6,  7,  7,  7,  7,  8,  8,  8,  9,  9,  9, 10,
  10, 10, 11, 11, 11, 12, 12, 13, 13, 13, 14, 14, 15, 15, 16, 16,
  17, 17, 18, 18, 19, 19, 20, 20, 21, 21, 22, 22, 23, 24, 24, 25,
  25, 26, 27, 27, 28, 29, 29, 30, 31, 32, 32, 33, 34, 35, 35, 36,
  37, 38, 39, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 50,
  51, 52, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 66, 67, 68,
  69, 70, 72, 73, 74, 75, 77, 78, 79, 81, 82, 83, 85, 86, 87, 89,
  90, 92, 93, 95, 96, 98, 99, 101, 102, 104, 105, 107, 109, 110, 112, 114,
  115, 117, 119, 120, 122, 124, 126, 127, 129, 131, 133, 135, 137, 138, 140, 142,
  144, 146, 148, 150, 152, 154, 156, 158, 160, 162, 164, 167, 169, 171, 173, 175,
  177, 180, 182, 184, 186, 189, 191, 193, 196, 198, 200, 203, 205, 208, 210, 213,
  215, 218, 220, 223, 225, 228, 231, 233, 236, 239, 241, 244, 247, 249, 252, 255
};

Adafruit_NeoPixel strip = Adafruit_NeoPixel(NUM_LEDS, LEDPIN, NEO_RGBW + NEO_KHZ800);
WiFiUDP Udp;
Ticker ticker;

unsigned int localPort = UDP_PORT;      // local port to listen for UDP packets
byte packetBuffer[MAX_UDP_PACKET_SIZE]; //buffer to hold incoming and outgoing packets
byte packetLedChangeTable[((NUM_LEDS) + 7) / 8];

void setup()
{
  Serial.begin (115200);
  while (!Serial) {
    ; // wait for serial port to connect. Needed for native USB port only
  }

  pinMode(BLUE_LED_PIN, OUTPUT);

  WiFiManager wifiManager;
  //wifiManager.resetSettings();   //reset settings - for testing
  wifiManager.setConfigPortalTimeout(180);
  wifiManager.setAPCallback(configModeCallback);
  wifiManager.setSTAStaticIPConfig(IPAddress(192,168,0,200), IPAddress(192,168,0,1), IPAddress(255,255,255,0));
  if(!wifiManager.autoConnect()){
    Serial.println("failed to connect, we should reset as see if it connects");
    delay(3000);
    ESP.reset();
    delay(5000);
  }
  Serial.println("connected...yeey :)");
  ticker.detach();

  strip.setBrightness(BRIGHTNESS);
  strip.begin();
  strip.show();

  Udp.begin(localPort);

  digitalWrite(BLUE_LED_PIN, HIGH); // turn off blue LED
}

//gets called when WiFiManager enters configuration mode
void configModeCallback (WiFiManager *myWiFiManager) {
  Serial.println("Entered config mode");
  Serial.println(WiFi.softAPIP());
  Serial.println(myWiFiManager->getConfigPortalSSID());    // if you used auto generated SSID, print it
  ticker.attach(0.2, tick);                                // entered config mode, make led toggle faster
} 

void tick()
{
  //toggle state
  int state = digitalRead(BLUE_LED_PIN);  // get the current state of GPIO1 pin
  digitalWrite(BLUE_LED_PIN, !state);     // set pin to the opposite state
}

void loop()
{
  
  while (Udp.parsePacket()) {
    int receivedCount = Udp.read(packetBuffer, MAX_UDP_PACKET_SIZE); // read the packet into the buffer
    if (receivedCount >= sizeof packetLedChangeTable) 
    {
      memcpy(packetLedChangeTable, packetBuffer, sizeof packetLedChangeTable);
      auto b = toBitBool(packetLedChangeTable);
      unsigned int bufferPos = sizeof packetLedChangeTable;
      int cr = 0;
      int cg = 0;
      int cb = 0;
      
      for (size_t i = 0; i < NUM_LEDS; i++)
      {
        if (b.get(i)) 
        {          
          if (receivedCount < (bufferPos + 3))
            break;          
          cr   = packetBuffer[bufferPos++];
          cg   = packetBuffer[bufferPos++];
          cb   = packetBuffer[bufferPos++];
          setNeoRGBW(i, cr, cg, cb);      
        }
      }
      strip.show();
      
    }
  }
}



int min(int a, int b)
{
  if (a < b) return a;
  return b;
}
int max(int a, int b)
{
  if (a > b) return a;
  return b;
}
float min(float a, float b)
{
  if (a < b) return a;
  return b;
}
float max(float a, float b)
{
  if (a > b) return a;
  return b;
}
int i_min0_max255(float a){
  if (a<0) return 0;
  if (a>255) return 255;
  return (int)a;
}

void setNeoRGBW(int pixel, int Ri, int Gi, int Bi){

  float tM = (float)(max(Ri, max(Gi, Bi)));
  
  //If the maximum value is 0, immediately return pure black.
  if (tM == 0)
  { 
    strip.setPixelColor(pixel, strip.Color(0,0,0,0));
    return;
  }

  //This section serves to figure out what the color with 100% hue is
  float multiplier = 255.0f / tM;
  float hR = (float)Ri * multiplier;
  float hG = (float)Gi * multiplier;
  float hB = (float)Bi * multiplier;

  //This calculates the Whiteness (not strictly speaking Luminance) of the color
  float M = max(hR, max(hG, hB));
  float m = min(hR, min(hG, hB));
  float Luminance = ((M + m) / 2.0f - 127.5f) * (255.0f / 127.5f) / multiplier;

  strip.setPixelColor(pixel, strip.Color(
    i_min0_max255(Gi - Luminance),
    i_min0_max255(Ri - Luminance),
    i_min0_max255(Bi - Luminance),
    neopix_gamma[i_min0_max255(Luminance)]));

}

uint8_t red(uint32_t c) {
  return (c >> 16);
}
uint8_t green(uint32_t c) {
  return (c >> 8);
}
uint8_t blue(uint32_t c) {
  return (c);
}
