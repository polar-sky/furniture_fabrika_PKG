using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tao.FreeGlut;
using Tao.OpenGl;
using Tao.DevIl;
using System.Drawing.Imaging;

namespace Kotova_Anna_PRI_118_KP
{
    public partial class Form1 : Form
    {
        double sizeX = 1, sizeY = 1, sizeZ = 1;
        double angle = 10, angleX = -82, angleY = 0, angleZ = -60; double translateX = -62, translateY = 13, translateZ = -15;
        float global_time = 0;
        double localTime;
        double cameraSpeed = 5;
        double chairX = 0;
        double chairY = 0;
        double chairZ = 0;
        float taburetPos = 0f;
        float boxX = 0f;
        float boxY = 0f;
        uint mGlTextureObject;
        uint mGlTextureObject2;
        uint mGlTextureObject3;
        uint mGlTextureObject4;
        uint mGlTextureObject5;
        uint mGlTextureObject6;
        uint mGlTextureObject7;
        int imageId;
        int imageId2;
        int imageId3;
        int imageId4;
        int imageId5;
        int imageId6;
        int imageId7;
        float ugol = 0.0f;
        private Explosion explosion = new Explosion(1, 10, 1, 300, 900);
        private Explosion explosionPortal = new Explosion(0, 0, 0, 20, 50);
        bool isExplosion = false;
        bool flagExplosion = false;
        float[] LightAmbient = { 1.0f, 1.0f, 1.0f, 1.0f };
        //float[] LightDiffuse = { 1.0f, 1.0f, 1.0f, 1.0f }; // Значения диффузного света 
        float[] LightPosition = { 1.0f, 1.0f, 1.0f, 1.0f };     // Позиция света
        private const int Level = 5;
        bool addChair = false;
        bool chairPresent = false;
        bool chairOnConveyor = false;
        bool chairInBox = false;
        //Высота и ширина для отрисовки
        private int _width = 400;
        private int _height = 400;
        //Bitmap для фрактала
        private Bitmap _fractal;
        // используем для отрисовки на PictureBox
        private Graphics _graph;
        uint fractalSerpiski;


        private void button1_Click(object sender, EventArgs e)
        {

            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_AMBIENT, LightAmbient);
            //Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_DIFFUSE, LightDiffuse);
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, LightPosition);
            Gl.glEnable(Gl.GL_LIGHT0);

            Gl.glFlush();
            AnT.Invalidate();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Gl.glDisable(Gl.GL_LIGHT0);

            Gl.glFlush();
            AnT.Invalidate();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            addChair = true;
            AnT.Focus();
            button3.Enabled = false;
        }

        public Form1()
        {
            InitializeComponent();
            AnT.InitializeContexts();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!chairOnConveyor)
            {
                ugol = ugol - 5.0f;
            }
            global_time += (float)RenderTimer.Interval / 1000;
            Draw();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // инициализация OpenGL
            // инициализация бибилиотеки glut
            Glut.glutInit();
            // инициализация режима экрана
            Glut.glutInitDisplayMode(Glut.GLUT_RGB | Glut.GLUT_DOUBLE | Glut.GLUT_DEPTH);

            comboBox1.SelectedIndex = 0;

            // цвет фона

            Gl.glClearColor(0.3f, 0.3f, 0.3f, 0.0f);
            Gl.glShadeModel(Gl.GL_SMOOTH);

            // рассчет освещения

            Gl.glEnable(Gl.GL_LIGHTING);

            Gl.glEnable(Gl.GL_DEPTH_TEST);      // Разрешить тест глубины

            Gl.glDepthFunc(Gl.GL_LEQUAL);       // Тип теста глубины

            Gl.glHint(Gl.GL_PERSPECTIVE_CORRECTION_HINT, Gl.GL_NICEST);

            // двухсторонний расчет освещения

            Gl.glLightModelf(Gl.GL_LIGHT_MODEL_TWO_SIDE, Gl.GL_TRUE);

            // автоматическое приведение нормалей к

            // единичной длине

            Gl.glEnable(Gl.GL_NORMALIZE);

            Il.ilInit();
            Il.ilEnable(Il.IL_ORIGIN_SET);

            // установка цвета очистки экрана (RGBA)
            //Gl.glClearColor(255, 255, 255, 1);

            // установка порта вывода
            Gl.glViewport(0, 0, AnT.Width, AnT.Height);

            // активация проекционной матрицы
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            // очистка матрицы
            Gl.glLoadIdentity();

            Glu.gluPerspective(60, (float)AnT.Width / (float)AnT.Height, 0.1, 800);

            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();

            Gl.glEnable(Gl.GL_DEPTH_TEST);

            //фрактал серпискина
            //создаем Bitmap для прямоугольника
            _fractal = new Bitmap(_width, _height);
            // cоздаем новый объект Graphics из указанного Bitmap
            _graph = Graphics.FromImage(_fractal);
            //создаем прямоугольник и вызываем функцию отрисовки ковра
            RectangleF carpet = new RectangleF(0, 0, _width, _height);
            DrawCarpet(Level, carpet);
            // идентификатор текстурного объекта


            // генерируем текстурный объект
            Gl.glGenTextures(1, out fractalSerpiski);

            // устанавливаем режим упаковки пикселей
            Gl.glPixelStorei(Gl.GL_UNPACK_ALIGNMENT, 1);

            // создаем привязку к только что созданной текстуре
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, fractalSerpiski);

            // устанавливаем режим фильтрации и повторения текстуры
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
            Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_REPLACE);

            BitmapData data = _fractal.LockBits(new System.Drawing.Rectangle(0, 0, _width, _height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGB, _width, _height, 0, Gl.GL_BGRA, Gl.GL_UNSIGNED_BYTE, data.Scan0);

            // ЗАГУРУЗКА ИЗОБРАЖЕНИЯ ПО УМОЛЧАНИЮ
            Il.ilGenImages(1, out imageId);
            Il.ilBindImage(imageId);
            if (Il.ilLoadImage("../../texture/pol.png"))
            {
                int width = Il.ilGetInteger(Il.IL_IMAGE_WIDTH);
                int height = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT);
                int bitspp = Il.ilGetInteger(Il.IL_IMAGE_BITS_PER_PIXEL);

                switch (bitspp)
                {
                    case 24:
                        mGlTextureObject = MakeGlTexture(Gl.GL_RGB, Il.ilGetData(), width, height);
                        break;
                    case 32:
                        mGlTextureObject = MakeGlTexture(Gl.GL_RGBA, Il.ilGetData(), width, height);
                        break;
                }
            }
            Il.ilDeleteImages(1, ref imageId2);
            Il.ilGenImages(1, out imageId2);
            Il.ilBindImage(imageId2);
            if (Il.ilLoadImage("../../texture/stena.png"))
            {
                int width = Il.ilGetInteger(Il.IL_IMAGE_WIDTH);
                int height = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT);
                int bitspp = Il.ilGetInteger(Il.IL_IMAGE_BITS_PER_PIXEL);
                switch (bitspp)
                {
                    case 24:
                        mGlTextureObject2 = MakeGlTexture(Gl.GL_RGB, Il.ilGetData(), width, height);
                        break;
                    case 32:
                        mGlTextureObject2 = MakeGlTexture(Gl.GL_RGBA, Il.ilGetData(), width, height);
                        break;
                }
            }

            Il.ilDeleteImages(1, ref imageId3);
            Il.ilGenImages(1, out imageId3);
            Il.ilBindImage(imageId3);
            if (Il.ilLoadImage("../../texture/plakat.png"))
            {
                int width = Il.ilGetInteger(Il.IL_IMAGE_WIDTH);
                int height = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT);
                int bitspp = Il.ilGetInteger(Il.IL_IMAGE_BITS_PER_PIXEL);
                switch (bitspp)
                {
                    case 24:
                        mGlTextureObject3 = MakeGlTexture(Gl.GL_RGB, Il.ilGetData(), width, height);
                        break;
                    case 32:
                        mGlTextureObject3 = MakeGlTexture(Gl.GL_RGBA, Il.ilGetData(), width, height);
                        break;
                }
            }

            Il.ilDeleteImages(1, ref imageId4);
            Il.ilGenImages(1, out imageId4);
            Il.ilBindImage(imageId4);
            if (Il.ilLoadImage("../../texture/cylinder.png"))
            {
                int width = Il.ilGetInteger(Il.IL_IMAGE_WIDTH);
                int height = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT);
                int bitspp = Il.ilGetInteger(Il.IL_IMAGE_BITS_PER_PIXEL);
                switch (bitspp)
                {
                    case 24:
                        mGlTextureObject4 = MakeGlTexture(Gl.GL_RGB, Il.ilGetData(), width, height);
                        break;
                    case 32:
                        mGlTextureObject4 = MakeGlTexture(Gl.GL_RGBA, Il.ilGetData(), width, height);
                        break;
                }
            }

            Il.ilDeleteImages(1, ref imageId5);
            Il.ilGenImages(1, out imageId5);
            Il.ilBindImage(imageId5);
            if (Il.ilLoadImage("../../texture/korobka.png"))
            {
                int width = Il.ilGetInteger(Il.IL_IMAGE_WIDTH);
                int height = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT);
                int bitspp = Il.ilGetInteger(Il.IL_IMAGE_BITS_PER_PIXEL);
                switch (bitspp)
                {
                    case 24:
                        mGlTextureObject5 = MakeGlTexture(Gl.GL_RGB, Il.ilGetData(), width, height);
                        break;
                    case 32:
                        mGlTextureObject5 = MakeGlTexture(Gl.GL_RGBA, Il.ilGetData(), width, height);
                        break;
                }
            }


            Il.ilDeleteImages(1, ref imageId6);
            Il.ilGenImages(1, out imageId6);
            Il.ilBindImage(imageId6);
            if (Il.ilLoadImage("../../texture/gosling.png"))
            {
                int width = Il.ilGetInteger(Il.IL_IMAGE_WIDTH);
                int height = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT);
                int bitspp = Il.ilGetInteger(Il.IL_IMAGE_BITS_PER_PIXEL);
                switch (bitspp)
                {
                    case 24:
                        mGlTextureObject6 = MakeGlTexture(Gl.GL_RGB, Il.ilGetData(), width, height);
                        break;
                    case 32:
                        mGlTextureObject6 = MakeGlTexture(Gl.GL_RGBA, Il.ilGetData(), width, height);
                        break;
                }
            }

            Il.ilDeleteImages(1, ref imageId7);
            Il.ilGenImages(1, out imageId7);
            Il.ilBindImage(imageId6);
            if (Il.ilLoadImage("../../texture/taburet.png"))
            {
                int width = Il.ilGetInteger(Il.IL_IMAGE_WIDTH);
                int height = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT);
                int bitspp = Il.ilGetInteger(Il.IL_IMAGE_BITS_PER_PIXEL);
                switch (bitspp)
                {
                    case 24:
                        mGlTextureObject7 = MakeGlTexture(Gl.GL_RGB, Il.ilGetData(), width, height);
                        break;
                    case 32:
                        mGlTextureObject7 = MakeGlTexture(Gl.GL_RGBA, Il.ilGetData(), width, height);
                        break;
                }
            }

            Il.ilDeleteImages(4, ref imageId2);


            RenderTimer.Start();

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void AnT_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.W)
            {
                translateY -= cameraSpeed;

            }
            if (e.KeyCode == Keys.S)
            {
                translateY += cameraSpeed;
            }
            if (e.KeyCode == Keys.A)
            {
                translateX += cameraSpeed;
            }
            if (e.KeyCode == Keys.D)
            {
                translateX -= cameraSpeed;

            }
            if (e.KeyCode == Keys.ControlKey)
            {
                translateZ += cameraSpeed;

            }
            if (e.KeyCode == Keys.Space)
            {
                translateZ -= cameraSpeed;
            }


            if (e.KeyCode == Keys.Q)
            {
                angleZ -= angle;
            }
            if (e.KeyCode == Keys.E)
            {
                angleZ += angle;
            }
            if (e.KeyCode == Keys.R)
            {
                angleX -= angle;
            }
            if (e.KeyCode == Keys.F)
            {
                angleX += angle;
            }
            if (e.KeyCode == Keys.Z)
            {
                sizeX += 0.1;
            }
            if (e.KeyCode == Keys.X)
            {
                sizeX -= 0.1;
            }

            //Конвейер вперед
            if (e.KeyCode == Keys.K)
            {
                if (taburetPos < 195)
                {
                    taburetPos += 1f;
                    ugol = ugol - 5.0f;
                }
            }
            //Конвейер назад
            if (e.KeyCode == Keys.L)
            {
                if (taburetPos > 0)
                {
                    taburetPos -= 1f;
                    ugol = ugol + 5.0f;
                }
            }

            //движение коробки
            if (e.KeyCode == Keys.NumPad8)
            {
                if (boxX >= -170)
                {
                boxX -= 1f;
                }
            }
            if (e.KeyCode == Keys.NumPad4)
            {
                if (boxY >= 0)
                {
                boxY -= 1f;
                }
            }
            if (e.KeyCode == Keys.NumPad2)
            {
                if (boxX <= 175)
                {
                boxX += 1f;
                }
            }
            if (e.KeyCode == Keys.NumPad6)
            {
                if (boxY <= 155 )
                {
                boxY += 1f;
                }
            }
        }

        private void Draw()
        {
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);

            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            // свойства материала

            float[] material_diffuse = { 1.0f, 1.0f, 1.0f, 1.0f };

            Gl.glMaterialfv(Gl.GL_FRONT_AND_BACK, Gl.GL_DIFFUSE, material_diffuse);

            Gl.glClearColor(0.3f, 0.3f, 0.3f, 0.0f);
            Gl.glLoadIdentity();
            Gl.glPushMatrix();
            Gl.glRotated(angleX, 1, 0, 0); Gl.glRotated(angleY, 0, 1, 0); Gl.glRotated(angleZ, 0, 0, 1);
            Gl.glTranslated(translateX, translateY, translateZ);
            Gl.glScaled(sizeX, sizeY, sizeZ);
            Gl.glPushMatrix();

            explosion.Calculate(global_time);
            if (isExplosion && !flagExplosion)
            {
                flagExplosion = true;
                explosion.SetNewPosition((float)chairX, (float)chairY, (float)chairZ + 50);
                explosion.SetNewPower(1000);
                explosion.Boooom(global_time, 200, 10);
            }

            Gl.glPushMatrix();

            /////////////////////
            // СВОЕОБРАЗНАЯ ЛЮСТРА
            /////////////////////
            Gl.glPushMatrix();
            Gl.glRotated(90, 1, 0, 0);
            Gl.glTranslated(0, 145, 0);
            Glut.glutSolidSphere(10, 16, 16);
            Gl.glPopMatrix();

            /////////////////////
            // ТАБУРЕТКА
            /////////////////////

            Gl.glPopMatrix();

            if (addChair)
            {
                Gl.glPushMatrix();

                //двигаем стул по конвейеру

                if (chairOnConveyor)
                {
                    Gl.glTranslated(0, 0 + taburetPos, 0);
                }

                if (chairZ <= -70)
                {
                    isExplosion = true;
                    //closeBox();
                }
                else
                {
                    Gl.glTranslated(70, -120, 70);
                    if (!chairPresent)
                    {
                        Gl.glTranslated(chairX, chairY, chairZ);
                        goslingTaburet();
                        chairPresent = true;
                    }
                    else
                    {
                        Gl.glTranslated(chairX, chairY, chairZ);
                        goslingTaburet();

                        if (chairY < 20)
                        {
                            chairY += 1.0;
                            Gl.glTranslated(chairX, chairY, chairZ);
                            if (chairY == 20.0) chairOnConveyor = true;

                        }

                        if (!chairInBox & taburetPos == 195)
                        {
                            chairZ -= 1.0;
                            Gl.glTranslated(chairX, chairY, chairZ);
                            if (chairZ <= -70)
                            {
                                chairInBox = true;
                                chairOnConveyor = false;
                            }
                        }
                    }
                }
                Gl.glPopMatrix();
            }

            /////////////////////
            // КОНВЕЙЕР
            /////////////////////

            Gl.glPushMatrix();
            Gl.glTranslated(-30, 0, 15);
            Gl.glRotated(90, 1, 0, 0);
            Gl.glRotated(90, 0, 1, 0);
            Gl.glTexCoord2f(1, 0);

            for (int i = 0; i <= 200; i += 10)
            {
                Gl.glTranslated(-10, 0, 0);

                Gl.glPushMatrix();
                Gl.glRotated(ugol, 0, 0, 1);

                Glut.glutSolidCylinder(5, 50, 7, 1);

                Gl.glPopMatrix();
            }

            Gl.glDisable(Gl.GL_BLEND);
            Gl.glDisable(Gl.GL_TEXTURE_2D);
            Gl.glDisable(Gl.GL_TEXTURE_GEN_S);
            Gl.glDisable(Gl.GL_TEXTURE_GEN_T);
            Gl.glDisable(Gl.GL_AUTO_NORMAL);
            Gl.glPopMatrix();

            /////////////////////
            // КОНВЕЙЕР НИЖНЯЯ ЧАСТЬ
            /////////////////////

            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, mGlTextureObject);
            Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_MODULATE);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);

            Gl.glTexCoord2f(1, 0);

            Gl.glTranslated(-4, -105, -20);
            Gl.glScaled(6, 20, 6);
            Gl.glColor3f(0.8f, 0.3f, 0.8f);
            Glut.glutSolidCube(10);


            Gl.glDisable(Gl.GL_BLEND);
            Gl.glDisable(Gl.GL_TEXTURE_2D);
            Gl.glPopMatrix();

            /////////////////////
            // ПОЛ
            /////////////////////
            ///

            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, mGlTextureObject);
            Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_MODULATE);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);

            Gl.glPushMatrix();
            Gl.glColor3f(1f / 255f * 21, 1f / 255f * 153, 1f / 255f * 37);
            Gl.glBegin(Gl.GL_TRIANGLE_FAN);
            Gl.glTexCoord2f(1, 0);
            Gl.glVertex3d(200, 200, -50);
            Gl.glTexCoord2f(0, 0);
            Gl.glVertex3d(-200, 200, -50);
            Gl.glTexCoord2f(0, 1);
            Gl.glVertex3d(-200, -200, -50);
            Gl.glTexCoord2f(1, 1);
            Gl.glVertex3d(200, -200, -50);
            Gl.glEnd();


            Gl.glDisable(Gl.GL_BLEND);
            Gl.glDisable(Gl.GL_TEXTURE_2D);
            Gl.glPopMatrix();


            /////////////////////
            // ЭТО ПОТОЛОК
            /////////////////////

            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, mGlTextureObject);
            Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_MODULATE);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);

            Gl.glPushMatrix();
            Gl.glColor3f(1f / 255f * 21, 1f / 255f * 153, 1f / 255f * 37);
            Gl.glBegin(Gl.GL_TRIANGLE_FAN);
            Gl.glTexCoord2f(1, 0);
            Gl.glVertex3d(200, 200, 150);
            Gl.glTexCoord2f(0, 0);
            Gl.glVertex3d(-200, 200, 150);
            Gl.glTexCoord2f(0, 1);
            Gl.glVertex3d(-200, -200, 150);
            Gl.glTexCoord2f(1, 1);
            Gl.glVertex3d(200, -200, 150);
            Gl.glEnd();
            Gl.glDisable(Gl.GL_BLEND);
            Gl.glDisable(Gl.GL_TEXTURE_2D);
            Gl.glPopMatrix();

            /////////////////////
            // СТЕНА С ФРАКТАЛОМ ПО ВАРИАНТУ
            /////////////////////

            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, fractalSerpiski);
            Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_MODULATE);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);

            Gl.glPushMatrix();
            Gl.glTranslated(-200, -200, -50);

            Gl.glBegin(Gl.GL_QUADS);
            Gl.glTexCoord2f(1, 0);
            Gl.glVertex3d(0, 0, 0);
            Gl.glTexCoord2f(0, 0);
            Gl.glVertex3d(400, 0, 0);
            Gl.glTexCoord2f(0, 1);
            Gl.glVertex3d(400, 0, 200);
            Gl.glTexCoord2f(1, 1);
            Gl.glVertex3d(0, 0, 200);
            Gl.glEnd();
            Gl.glDisable(Gl.GL_BLEND);
            Gl.glDisable(Gl.GL_TEXTURE_2D);
            Gl.glPopMatrix();

            /////////////////////
            // СТЕНА 
            /////////////////////

            Gl.glPushMatrix();
            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, mGlTextureObject2);
            Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_MODULATE);

            Gl.glTranslated(-200, -200, -50);

            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glTexCoord2f(1, 0);
            Gl.glVertex3d(0, 0, 0);
            Gl.glTexCoord2f(0, 0);
            Gl.glVertex3d(0, 400, 0);
            Gl.glTexCoord2f(0, 1);
            Gl.glVertex3d(0, 400, 200);
            Gl.glTexCoord2f(1, 1);
            Gl.glVertex3d(0, 0, 200);
            Gl.glEnd();


            Gl.glBegin(Gl.GL_QUADS);
            Gl.glTexCoord2f(1, 0);
            Gl.glVertex3d(0, 400, 0);
            Gl.glTexCoord2f(0, 0);
            Gl.glVertex3d(400, 400, 0);
            Gl.glTexCoord2f(0, 1);
            Gl.glVertex3d(400, 400, 200);
            Gl.glTexCoord2f(1, 1);
            Gl.glVertex3d(0, 400, 200);
            Gl.glEnd();

            Gl.glDisable(Gl.GL_BLEND);
            Gl.glDisable(Gl.GL_TEXTURE_2D);
            Gl.glPopMatrix();

            /////////////////////
            // КОРОБКА
            /////////////////////
            ///
            Gl.glPushMatrix();
            if (flagExplosion == true)
            {
                closeBox();
                Gl.glTranslated(0 + boxX, 0 + boxY, 0);
            }

            Gl.glPushMatrix();
            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, mGlTextureObject5);
            Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_MODULATE);

            Gl.glTranslated(-28, -4, -50);
            Gl.glScaled(0.5, 0.5, 0.5);

            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glTexCoord2f(1, 0);
            Gl.glVertex3d(0, 0, 0);
            Gl.glTexCoord2f(0, 0);
            Gl.glVertex3d(0, 100, 0);
            Gl.glTexCoord2f(0, 1);
            Gl.glVertex3d(0, 100, 100);
            Gl.glTexCoord2f(1, 1);
            Gl.glVertex3d(0, 0, 100);
            Gl.glEnd();


            Gl.glBegin(Gl.GL_QUADS);
            Gl.glTexCoord2f(1, 0);
            Gl.glVertex3d(0, 100, 0);
            Gl.glTexCoord2f(0, 0);
            Gl.glVertex3d(100, 100, 0);
            Gl.glTexCoord2f(0, 1);
            Gl.glVertex3d(100, 100, 100);
            Gl.glTexCoord2f(1, 1);
            Gl.glVertex3d(0, 100, 100);
            Gl.glEnd();

            Gl.glBegin(Gl.GL_QUADS);
            Gl.glTexCoord2f(1, 0);
            Gl.glVertex3d(100, 100, 0);
            Gl.glTexCoord2f(0, 0);
            Gl.glVertex3d(100, 0, 0);
            Gl.glTexCoord2f(0, 1);
            Gl.glVertex3d(100, 0, 100);
            Gl.glTexCoord2f(1, 1);
            Gl.glVertex3d(100, 100, 100);
            Gl.glEnd();

            Gl.glBegin(Gl.GL_QUADS);
            Gl.glTexCoord2f(1, 0);
            Gl.glVertex3d(100, 100, 0);
            Gl.glTexCoord2f(0, 0);
            Gl.glVertex3d(100, 0, 0);
            Gl.glTexCoord2f(0, 1);
            Gl.glVertex3d(100, 0, 100);
            Gl.glTexCoord2f(1, 1);
            Gl.glVertex3d(100, 100, 100);
            Gl.glEnd();

            Gl.glBegin(Gl.GL_QUADS);
            Gl.glTexCoord2f(1, 0);
            Gl.glVertex3d(100, 0, 0);
            Gl.glTexCoord2f(0, 0);
            Gl.glVertex3d(0, 0, 0);
            Gl.glTexCoord2f(0, 1);
            Gl.glVertex3d(0, 0, 100);
            Gl.glTexCoord2f(1, 1);
            Gl.glVertex3d(100, 0, 100);
            Gl.glEnd();

            //ДНО КОРОБКИ

            Gl.glBegin(Gl.GL_QUADS);
            Gl.glTexCoord2f(1, 0);
            Gl.glVertex3d(0, 0, 1);
            Gl.glTexCoord2f(0, 0);
            Gl.glVertex3d(0, 100, 1);
            Gl.glTexCoord2f(0, 1);
            Gl.glVertex3d(100, 100, 1);
            Gl.glTexCoord2f(1, 1);
            Gl.glVertex3d(100, 0, 1);
            Gl.glEnd();

            Gl.glDisable(Gl.GL_BLEND);
            Gl.glDisable(Gl.GL_TEXTURE_2D);
            Gl.glPopMatrix();
            Gl.glPopMatrix();

            /////////////////////
            // ПЛАКАТ 2
            /////////////////////
            Gl.glPushMatrix();
            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, mGlTextureObject6);
            Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_MODULATE);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);

            Gl.glTranslated(-200, -200, -50);

            Gl.glBegin(Gl.GL_QUADS);
            Gl.glTexCoord2f(1, 0);
            Gl.glVertex3d(1, 150, 50);
            Gl.glTexCoord2f(0, 0);
            Gl.glVertex3d(1, 250, 50);
            Gl.glTexCoord2f(0, 1);
            Gl.glVertex3d(1, 250, 100);
            Gl.glTexCoord2f(1, 1);
            Gl.glVertex3d(1, 150, 100);
            Gl.glEnd();

            Gl.glDisable(Gl.GL_BLEND);
            Gl.glDisable(Gl.GL_TEXTURE_2D);
            Gl.glPopMatrix();

            /////////////////////
            // ПЛАКАТ 1
            /////////////////////
            Gl.glPushMatrix();
            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, mGlTextureObject3);
            Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_MODULATE);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);

            Gl.glTranslated(-200, -200, -50);

            Gl.glBegin(Gl.GL_QUADS);
            Gl.glTexCoord2f(1, 0);
            Gl.glVertex3d(50, 399, 50);
            Gl.glTexCoord2f(0, 0);
            Gl.glVertex3d(100, 399, 50);
            Gl.glTexCoord2f(0, 1);
            Gl.glVertex3d(100, 399, 150);
            Gl.glTexCoord2f(1, 1);
            Gl.glVertex3d(50, 399, 150);
            Gl.glEnd();

            Gl.glDisable(Gl.GL_BLEND);
            Gl.glDisable(Gl.GL_TEXTURE_2D);
            Gl.glPopMatrix();

            // возвращаем состояние матрицы
            Gl.glPopMatrix();
            // отрисовываем геометрию
            Gl.glFlush();

            // обновляем состояние элемента
            AnT.Invalidate();

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                //Главный вид
                case 0:
                    angle = 10; angleX = -75; angleY = 0; angleZ = -90;
                    //400, 0, 0
                    translateX = -350; translateY = 0; translateZ = -100;
                    break;
                //Конвейер
                case 1:
                    angle = 10; angleX = -75; angleY = 0; angleZ = -180;
                    translateX = 0; translateY = -180; translateZ = -70;
                    break;
                //Угловая камера
                case 2:
                    angle = -50; angleX = -60; angleY = 0; angleZ = -55;
                    translateX = -200; translateY = 150; translateZ = -150;
                    break;
            }
            AnT.Focus();
        }

        private static uint MakeGlTexture(int Format, IntPtr pixels, int w, int h)
        {

            // идентификатор текстурного объекта
            uint texObject;

            // генерируем текстурный объект
            Gl.glGenTextures(1, out texObject);

            // устанавливаем режим упаковки пикселей
            Gl.glPixelStorei(Gl.GL_UNPACK_ALIGNMENT, 1);

            // создаем привязку к только что созданной текстуре
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, texObject);

            // устанавливаем режим фильтрации и повторения текстуры
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
            Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_REPLACE);

            // создаем RGB или RGBA текстуру
            switch (Format)
            {

                case Gl.GL_RGB:
                    Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGB, w, h, 0, Gl.GL_RGB, Gl.GL_UNSIGNED_BYTE, pixels);
                    break;

                case Gl.GL_RGBA:
                    Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA, w, h, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, pixels);
                    break;

            }

            // возвращаем идентификатор текстурного объекта

            return texObject;

        }

        private void DrawCarpet(int level, RectangleF carpet)
        {
            //проверяем, закончили ли мы построение
            if (level == 0)
            {
                //Рисуем прямоугольник
                _graph.FillRectangle(Brushes.OrangeRed, carpet);
            }
            else
            {
                // делим прямоугольник на 9 частей
                var width = carpet.Width / 3f;
                var height = carpet.Height / 3f;
                // (x1, y1) - координаты левой верхней вершины прямоугольника
                // от нее будем отсчитывать остальные вершины маленьких прямоугольников
                var x1 = carpet.Left;
                var x2 = x1 + width;
                var x3 = x1 + 2f * width;

                var y1 = carpet.Top;
                var y2 = y1 + height;
                var y3 = y1 + 2f * height;

                DrawCarpet(level - 1, new RectangleF(x1, y1, width, height)); // левый 1(верхний)
                DrawCarpet(level - 1, new RectangleF(x2, y1, width, height)); // средний 1
                DrawCarpet(level - 1, new RectangleF(x3, y1, width, height)); // правый 1
                DrawCarpet(level - 1, new RectangleF(x1, y2, width, height)); // левый 2
                DrawCarpet(level - 1, new RectangleF(x3, y2, width, height)); // правый 2
                DrawCarpet(level - 1, new RectangleF(x1, y3, width, height)); // левый 3
                DrawCarpet(level - 1, new RectangleF(x2, y3, width, height)); // средний 3
                DrawCarpet(level - 1, new RectangleF(x3, y3, width, height)); // правый 3
            }
        }

        private void goslingTaburet()
        {
            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, mGlTextureObject7);
            Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_MODULATE);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);

            Gl.glPushMatrix();
            Gl.glTranslated(-100, -100, -50);

            //СЕДУШКА
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glVertex3d(0, 0, 50);
            Gl.glVertex3d(0, 50, 50);
            Gl.glVertex3d(50, 50, 50);
            Gl.glVertex3d(50, 0, 50);
            Gl.glEnd();

            Gl.glBegin(Gl.GL_QUADS);
            Gl.glTexCoord2f(1, 0);
            Gl.glVertex3d(0, 0, 55);
            Gl.glTexCoord2f(0, 0);
            Gl.glVertex3d(0, 50, 55);
            Gl.glTexCoord2f(0, 1);
            Gl.glVertex3d(50, 50, 55);
            Gl.glTexCoord2f(1, 1);
            Gl.glVertex3d(50, 0, 55);
            Gl.glEnd();

            Gl.glBegin(Gl.GL_QUADS);
            Gl.glVertex3d(0, 0, 50);
            Gl.glVertex3d(0, 50, 50);
            Gl.glVertex3d(0, 50, 55);
            Gl.glVertex3d(0, 0, 55);
            Gl.glEnd();

            Gl.glBegin(Gl.GL_QUADS);
            Gl.glVertex3d(50, 0, 50);
            Gl.glVertex3d(50, 50, 50);
            Gl.glVertex3d(50, 50, 55);
            Gl.glVertex3d(50, 0, 55);
            Gl.glEnd();

            Gl.glBegin(Gl.GL_QUADS);
            Gl.glVertex3d(50, 50, 50);
            Gl.glVertex3d(50, 50, 55);
            Gl.glVertex3d(0, 50, 55);
            Gl.glVertex3d(0, 50, 50);
            Gl.glEnd();

            Gl.glBegin(Gl.GL_QUADS);
            Gl.glVertex3d(50, 50, 50);
            Gl.glVertex3d(50, 50, 55);
            Gl.glVertex3d(0, 50, 55);
            Gl.glVertex3d(0, 50, 50);
            Gl.glEnd();

            Gl.glBegin(Gl.GL_QUADS);
            Gl.glVertex3d(50, 0, 50);
            Gl.glVertex3d(0, 0, 50);
            Gl.glVertex3d(0, 0, 55);
            Gl.glVertex3d(50, 0, 55);
            Gl.glEnd();

            //КРАСИВЫЕ НОЖКИ
            Gl.glTranslated(45, 5, 0);
            Glut.glutSolidCylinder(2, 50, 10, 10);
            Gl.glTranslated(0, 40, 0);
            Glut.glutSolidCylinder(2, 50, 10, 10);
            Gl.glTranslated(-40, 0, 0);
            Glut.glutSolidCylinder(2, 50, 10, 10);
            Gl.glTranslated(0, -40, 0);
            Glut.glutSolidCylinder(2, 50, 10, 10);

            Gl.glDisable(Gl.GL_BLEND);
            Gl.glDisable(Gl.GL_TEXTURE_2D);
            Gl.glPopMatrix();
        }

        private void closeBox()
        {
            Gl.glPushMatrix();
            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, mGlTextureObject5);
            Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_MODULATE);

            Gl.glTranslated(-28 + boxX, -4 + boxY, -50);
            Gl.glScaled(0.5, 0.5, 0.5);

            Gl.glBegin(Gl.GL_QUADS);
            Gl.glTexCoord2f(1, 0);
            Gl.glVertex3d(0, 0, 100);
            Gl.glTexCoord2f(0, 0);
            Gl.glVertex3d(0, 100, 100);
            Gl.glTexCoord2f(0, 1);
            Gl.glVertex3d(100, 100, 100);
            Gl.glTexCoord2f(1, 1);
            Gl.glVertex3d(100, 0, 100);
            Gl.glEnd();

            Gl.glDisable(Gl.GL_BLEND);
            Gl.glDisable(Gl.GL_TEXTURE_2D);
            Gl.glPopMatrix();
        }

    }
}
