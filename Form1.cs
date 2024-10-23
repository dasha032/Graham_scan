﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Graham_scan
{
    public partial class Form1 : Form
    {
        List<PointF> points = new List<PointF>();
        List<PointF> hull = new List<PointF>();
        private int selectedPointIndex = -1;
        bool buildingHull = false;
        bool moving = false;
        public Form1()
        {
            InitializeComponent();
            
        }
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            DrawPoints(g);
            if (hull.Count > 0)
                DrawHull(g);
        }
        private void DrawPoints(Graphics g)
        {
            foreach (PointF p in points)
            {
                g.FillEllipse(Brushes.IndianRed, p.X - 2, p.Y - 2, 7, 7);
            }
            
            if (hull.Count() > 1)
            { 
                for (int i = 0; i < hull.Count;i++)
                {
                    g.FillEllipse(Brushes.LightSeaGreen, hull[i].X - 4, hull[i].Y - 4, 10, 10);

                }
            }

        }
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                
                for (int i = 0; i < points.Count; i++)
                {
                    if (Math.Abs(e.X - points[i].X) < 5 && Math.Abs(e.Y - points[i].Y) < 5)
                    {
                        selectedPointIndex = i;
                        return;
                    }
                }
                points.Add(new Point(e.X, e.Y));
            }    
                
            else
            {
                for (int i = 0; i < points.Count; i++)
                {
                    if (Math.Abs(e.X - points[i].X) < 5 && Math.Abs(e.Y - points[i].Y) < 5)
                    {
                        points.RemoveAt(i);
                        break;
                    }
                }
                if (buildingHull)
                     hull = Graham_Scan();
            }
            pictureBox1.Invalidate();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //selectedPointIndex = -1;
            if (points.Count == 0)
            {
                MessageBox.Show("Недостаточно точек для построения");
                return;
            }
            buildingHull = true;
            hull = Graham_Scan();            
            pictureBox1.Invalidate();
        }
        private void DrawHull(Graphics g)
        {
            Pen pen = new Pen(Color.Blue, 5/2);
            
            if (hull.Count() > 1)
            {
                for (int i = 0; i <= hull.Count() - 2; i++)
                {
                    g.DrawLine(pen, hull[i], hull[i + 1]);
                }
                g.DrawLine(pen, hull[hull.Count() - 1], hull[0]);
            }
           
            
        }
        private List<PointF> Graham_Scan()
        {
            if (points.Count == 1)
            {
                hull.Add(points[0]);
                return hull;
            }
             
            int minIndex = 0;
            for (int i = 1; i < points.Count(); i++)
            {
                if (points[i].Y > points[minIndex].Y || points[i].Y == points[minIndex].Y && points[i].X < points[minIndex].X)
                {
                    minIndex = i;
                }
            }
            Swap(points, 0, minIndex);
            PointF p0 = points[0];
            for (int i = 2; i < points.Count; i++)
            {
                int j = i;
                while (j > 1 && Rotate(p0, points[j - 1], points[j]) < 0)
                {
                    Swap(points, j, j - 1);
                    j--;
                }
            }
            hull.Clear();
            hull.Add(points[0]);
            hull.Add(points[1]);


            // Проходим по оставшимся точкам
            for (int i = 2; i < points.Count(); i++)
            {
                // Удаляем вершины из стека, пока они образуют поворот вправо
                while (hull.Count() > 1 && Rotate(hull[hull.Count() - 2], hull[hull.Count() - 1], points[i]) <= 0)
                {
                    hull.RemoveAt(hull.Count() - 1);
                }
                // Добавляем текущую точку в стек
                hull.Add(points[i]);
            }
            return hull;

        }
        private double Rotate(PointF a, PointF b, PointF c)
        {
            return (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X);
        }
        private void Swap(List<PointF> points, int i, int j)
        {
            PointF temp = points[i];
            points[i] = points[j];
            points[j] = temp;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            buildingHull = false;
            selectedPointIndex = -1;
            points.Clear();
            hull.Clear();
            pictureBox1.Invalidate();
        }
        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (selectedPointIndex >= 0 && e.Button == MouseButtons.Left)
            {
                moving = true;
                PointF oldpoint = points[selectedPointIndex];
                // Перемещаем выбранную точку
                points[selectedPointIndex] = new PointF(e.X, e.Y);
                if (buildingHull /*&& hull.Contains(oldpoint)*/)
                    hull = Graham_Scan();  
                    //hull = UpdateHullAfterMove(points[selectedPointIndex]);
                pictureBox1.Invalidate(); // Перерисовать PictureBox
                moving = false;
            }
        }
        private List<PointF> UpdateHullAfterMove(PointF movedPoint)
        {
            // Найти индекс перемещаемой точки в текущей оболочке
            int indexInHull = hull.IndexOf(movedPoint);

            // Если точка была на оболочке
            if (indexInHull != -1)
            {
                // Удалить точку из оболочки
                hull.RemoveAt(indexInHull);

                // Добавить новую позицию перемещаемой точки
                hull.Add(points[selectedPointIndex]);

                // Пересчитать выпуклую оболочку с учётом новой точки
                hull = Graham_Scan();
            }

            // Вернуть обновлённую оболочку
            return hull;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
           
            selectedPointIndex = -1; // Сбрасываем выбранную точку
        }

    }
}