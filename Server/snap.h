#define _CRT_SECURE_NO_WARNINGS

using namespace std;

static unsigned short read_u16(FILE* fp);
static unsigned int   read_u32(FILE* fp);
static int            read_s32(FILE* fp);

static unsigned short read_u16(FILE* fp)
{
    unsigned char b0, b1;

    b0 = getc(fp);
    b1 = getc(fp);

    return ((b1 << 8) | b0);
}


static unsigned int read_u32(FILE* fp)
{
    unsigned char b0, b1, b2, b3;

    b0 = getc(fp);
    b1 = getc(fp);
    b2 = getc(fp);
    b3 = getc(fp);

    return ((((((b3 << 8) | b2) << 8) | b1) << 8) | b0);
}


static int read_s32(FILE* fp)
{
    unsigned char b0, b1, b2, b3;

    b0 = getc(fp);
    b1 = getc(fp);
    b2 = getc(fp);
    b3 = getc(fp);

    return ((int)(((((b3 << 8) | b2) << 8) | b1) << 8) | b0);
}

class Snap {
private:
    char* path;

    typedef struct
    {
        unsigned int    bfType;
        unsigned long   bfSize;
        unsigned int    bfReserved1;
        unsigned int    bfReserved2;
        unsigned long   bfOffBits;
    } BITMAPFILEHEADER;

    typedef struct
    {
        unsigned int    biSize;
        int             biWidth;
        int             biHeight;
        unsigned short  biPlanes;
        unsigned short  biBitCount;
        unsigned int    biCompression;
        unsigned int    biSizeImage;
        int             biXPelsPerMeter;
        int             biYPelsPerMeter;
        unsigned int    biClrUsed;
        unsigned int    biClrImportant;
    } BITMAPINFOHEADER;

    typedef struct
    {
        int   rgbBlue;
        int   rgbGreen;
        int   rgbRed;
        int   rgbReserved;
    } RGBQUAD;

    int numToDv(int color) {
        if (0 <= color && color < 64) {
            return 0b00;
        }
        else if (63 < color && color < 128) {
            return 0b01;
        }
        else if (127 < color && color < 192) {
            return 0b10;
        }
        else if (191 < color && color < 256) {
            return 0b11;
        }
        else {
            return -1;
        }
    }

    uint32_t* analysis(char pathToFile[256]) //reading BMP file
    {
        FILE* pFile = fopen(pathToFile, "rb");

        BITMAPFILEHEADER header;

        header.bfType = read_u16(pFile);
        header.bfSize = read_u32(pFile);
        header.bfReserved1 = read_u16(pFile);
        header.bfReserved2 = read_u16(pFile);
        header.bfOffBits = read_u32(pFile);

        BITMAPINFOHEADER bmiHeader;

        bmiHeader.biSize = read_u32(pFile);
        bmiHeader.biWidth = read_s32(pFile);
        bmiHeader.biHeight = read_s32(pFile);
        bmiHeader.biPlanes = read_u16(pFile);
        bmiHeader.biBitCount = read_u16(pFile);
        bmiHeader.biCompression = read_u32(pFile);
        bmiHeader.biSizeImage = read_u32(pFile);
        bmiHeader.biXPelsPerMeter = read_s32(pFile);
        bmiHeader.biYPelsPerMeter = read_s32(pFile);
        bmiHeader.biClrUsed = read_u32(pFile);
        bmiHeader.biClrImportant = read_u32(pFile);

        RGBQUAD** rgb = new RGBQUAD * [bmiHeader.biWidth];
        for (int i = 0; i < bmiHeader.biWidth; i++) {
            rgb[i] = new RGBQUAD[bmiHeader.biHeight];
        }

        double* colors = new double[64];
        for (int i = 0; i < 64; i++) {
            colors[i] = 0;
        }

        int index;

        for (int i = 0; i < bmiHeader.biWidth; i++) {
            for (int j = 0; j < bmiHeader.biHeight; j++) {
                index = 0;
                rgb[i][j].rgbBlue = getc(pFile);
                index += numToDv(rgb[i][j].rgbBlue) << 4;
                rgb[i][j].rgbGreen = getc(pFile);
                index += numToDv(rgb[i][j].rgbGreen) << 2;
                rgb[i][j].rgbRed = getc(pFile);
                index += numToDv(rgb[i][j].rgbRed);
                colors[index]++;
            }
            getc(pFile);
        }

        uint32_t* array = new uint32_t[3];

        array[0] = (uint32_t)colors;
        array[1] = bmiHeader.biHeight;
        array[2] = bmiHeader.biWidth;

        fclose(pFile);

        delete []rgb;

        return array;
    }

    double distance(double arr_1[64], double arr_2[64]) { //calculating the vector distance of images
        int dis = 0;
        for (int i = 0; i < 64; i++) {
            dis += (arr_1[i] - arr_2[i]) * ((arr_1[i] - arr_2[i]));
        }
        if (dis < 0) {
            return 0;
        }
        return sqrt(dis);
    }
public:

    Snap(const char* pathToBeer) {
        path = (char*)pathToBeer;
    }

    bool compare() { //comparing images and returning the result of finding Pivo on the image
        char pathToOriginalBeer[] = "C:/Users/wodge/pivo.bmp";

        uint32_t* arr_1 = analysis(pathToOriginalBeer);
        uint32_t* arr_2 = analysis(Snap::path);

        int width[2];
        width[0] = arr_1[2];
        width[1] = arr_2[2];

        int height[2];
        height[0] = arr_1[1];
        height[1] = arr_2[1];

        double* arr1 = (double*)arr_1[0];
        double* arr2 = (double*)arr_2[0];

        return (distance(arr1, arr2) <= 5000);
    }

};