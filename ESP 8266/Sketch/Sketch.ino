#include <BitBool.h>
#include <Math.h>

#include <WiFiUdp.h>
#include <WiFiServer.h>
#include <WiFiClientSecure.h>
#include <WiFiClient.h>
#include <ESP8266WiFiType.h>
#include <ESP8266WiFiSTA.h>
#include <ESP8266WiFiScan.h>
#include <ESP8266WiFiMulti.h>
#include <ESP8266WiFiGeneric.h>
#include <ESP8266WiFiAP.h>
#include <ESP8266WiFi.h>
#include <Adafruit_NeoPixel.h>

#define LEDPIN D4             // Neopixel Pin ( hier wurde der digitalpin f�r die Pixel angeschlossen! )
#define NUM_LEDS 60
#define BRIGHTNESS 50        // Helligkeit hardcoded
#define MAX_UDP_PACKET_SIZE 255
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

const char* ssid     = "xxxxxxxxxxxxxxxxxxx";
const char* password = "yyyyyyyyyyyyyyyyyyy";

unsigned int localPort = UDP_PORT;      // local port to listen for UDP packets
byte packetBuffer[MAX_UDP_PACKET_SIZE]; //buffer to hold incoming and outgoing packets
byte packetLedChangeTable[((NUM_LEDS) + 7) / 8];

void setup()
{
  Serial.begin (115200);
  while (!Serial) {
    ; // wait for serial port to connect. Needed for native USB port only
  }

  IPAddress ip(192,168,10,200);
  IPAddress gateway(192,168,10,1);
  IPAddress subnet(255,255,255,0);
  
  Serial.println("Startup...");
  Serial.print("Connecting to ");
  Serial.println(ssid);

  WiFi.mode(WIFI_STA);
  WiFi.config(ip, gateway, subnet);
  WiFi.begin(ssid, password);

  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }

  Serial.println("");
  Serial.println("WiFi connected");
  Serial.println("IP address: ");
  Serial.println(WiFi.localIP());

  strip.setBrightness(BRIGHTNESS);
  strip.begin();
  strip.show();

  Udp.begin(localPort);
}

void loop()
{
  
  while (Udp.parsePacket()) {
    int receivedCount = Udp.read(packetBuffer, MAX_UDP_PACKET_SIZE); // read the packet into the buffer
    if (receivedCount > 8) 
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
        }
        setNeoRGBW(i, cr, cg, cb);      
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