using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using Color = System.Drawing.Color;
using Matrix = System.Drawing.Drawing2D.Matrix;
using Pen = System.Drawing.Pen;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;

namespace LandscapePostcard
{
    public partial class Form1 : Form
    {
        int PointCount = 8;
        public List<lPoint> Points;
        public List<lPoint> PerspectivePoints;
        public List<Triangle> PerspectiveTriangles;
        public double DiagramWidth => 500;
        public double DiagramHeight => 500;
        public List<Triangle> trList;
        public int numForSequence = 0;

        public readonly List<Color> segmentLightColors = new List<Color>
        {Color.DarkSeaGreen, Color.PaleGreen, Color.Olive,
            Color.BlanchedAlmond, Color.Khaki, Color.Firebrick };
        public readonly List<Color> segmentShadowColors = new List<Color>
        {Color.Green, Color.SeaGreen, Color.DarkOliveGreen, Color.Moccasin,
            Color.DarkKhaki, Color.Maroon};
        public readonly List<Color> leafLightColors = new List<Color>
        { Color.LightGreen, Color.MediumAquamarine, Color.Gold, Color.Crimson };
        public readonly List<Color> leafShadowColors = new List<Color>
        { Color.MediumSeaGreen, Color.Teal, Color.Goldenrod, Color.DarkRed };
        public readonly List<Color> grainColors = new List<Color>
        { Color.Tan, Color.Wheat, Color.Goldenrod, Color.DarkGoldenrod, Color.SandyBrown };
        public readonly List<Color> berryLightColors = new List<Color>
        { Color.HotPink, Color.MediumTurquoise, Color.Crimson,
            Color.SeaShell, Color.IndianRed, Color.MediumBlue };
        public readonly List<Color> berryShadowColors = new List<Color>
        { Color.MediumVioletRed, Color.Teal, Color.Brown,
            Color.OldLace, Color.DarkRed, Color.DarkBlue };
        public readonly List<Color> flowerPetal1Colors = new List<Color>
        { Color.Yellow,Color.LightYellow, Color.Crimson,
            Color.PaleVioletRed, Color.Thistle, Color.DeepSkyBlue };
        public readonly List<Color> flowerPetal2Colors = new List<Color>
        { Color.Orange, Color.Yellow, Color.DarkRed,
            Color.Pink, Color.Snow, Color.AliceBlue };
        public readonly List<Color> flowerCenter1Colors = new List<Color>
        { Color.Yellow, Color.LightBlue, Color.GhostWhite,
            Color.FloralWhite, Color.NavajoWhite };
        public readonly List<Color> flowerCenter2Colors = new List<Color>
        { Color.Snow, Color.AliceBlue, Color.Khaki, Color.Bisque, Color.LemonChiffon };
        int Xoffset = 60;
        static Random rnd = new Random();
        public Form1()
        {
            InitializeComponent();
            GraphicsPath p = new GraphicsPath();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            //pictureBox1.Image = null;

        }

        private void DrawFlower(Graphics g, Plant.Segment segm, Plant p,
            Plant.Flower fl, List<Color> palette)
        {
            float lcoef;
            int yOffset;

            lcoef = Convert.ToSingle(1 + rnd.NextDouble());
            yOffset = (int)(fl.width - (fl.width / 2 * lcoef));

            // CHANGING TO ZERO SLIGHTLY CHANGES FLOWER PERSPECTIVE.

            //yOffset = 0;

            GraphicsPath bigEl = new GraphicsPath();
            bigEl.AddEllipse(segm.finish.X - (fl.width * lcoef) / 2,
                segm.finish.Y - fl.width / 2 + yOffset,
                fl.width, fl.width);
            GraphicsPath smallEl = new GraphicsPath();
            smallEl.AddEllipse(segm.finish.X - fl.sWidth * lcoef / 2,
                segm.finish.Y - fl.sWidth / 2, fl.sWidth, fl.sWidth);

            PointF[] bigArr = bigEl.PathPoints;
            PointF[] smallArr = smallEl.PathPoints;
            GraphicsPath petals = new GraphicsPath();
            if (fl.petalsNum < 6)
            {
                for (int i = 0; i < bigArr.Length - 1; i += fl.petalsNum - 1)
                {
                    //petals.AddLine(bigArr[i], smallArr[i]);
                    petals.AddCurve(new PointF[] { smallArr[i], bigArr[i + 1],
                        smallArr[i + 2] }, 2);
                }
            }
            else
            {
                for (int i = 0; i < bigArr.Length - 2; i += 1)
                {
                    //petals.AddLine(bigArr[i], smallArr[i]);
                    petals.AddCurve(new PointF[] { smallArr[i], bigArr[i + 1],
                        smallArr[i + 2] }, 2);
                }
            }
            Matrix m = new Matrix();
            m.Translate(segm.finish.X, segm.finish.Y);
            m.Scale(lcoef, 1);
            m.Translate(-segm.finish.X, -segm.finish.Y);
            bigEl.Transform(m);
            smallEl.Transform(m);
            petals.Transform(m);
            Matrix m1 = new Matrix();
            m1.Translate(segm.finish.X, segm.finish.Y);
            m1.Rotate(rnd.Next(-90, 90));
            m1.Translate(-segm.finish.X, -segm.finish.Y);
            bigEl.Transform(m1);
            smallEl.Transform(m1);
            petals.Transform(m1);


            //g.DrawPath(new Pen(Color.Red, 2f), bigEl);
            //g.DrawPath(new Pen(Color.Red, 2f), smallEl);
            //g.DrawPath(new Pen(Color.Red, 2f), petals);
            PathGradientBrush pgb = new PathGradientBrush(petals);
            PathGradientBrush pgb2 = new PathGradientBrush(smallEl);
            Color[] colors = new Color[] { palette[4] };
            Color[] colors2 = new Color[] { palette[6] };
            pgb.SurroundColors = colors;
            pgb.CenterColor = palette[5];
            pgb2.SurroundColors = colors2;
            pgb2.CenterColor = palette[7];
            g.FillPath(pgb, petals);
            g.FillPath(pgb2, smallEl);
            //g.FillPath(new SolidBrush(Color.Yellow), smallEl);
        }

        private void DrawGrain(Graphics g, Plant.Segment segm,
            Plant p, List<Color> palette)
        {
            int grainAngle = rnd.Next(10, 60);
            // = new Plant.Segment(segm.dir, 10, segm.finish, 5);
            Plant.Segment s = p.CreateSegment(segm, 1f, 0);
            for (int i = 0; i < 10; i++)
            {

                Plant.Leaf l = new Plant.Leaf(10, s.dir, new Point(s.finish.X * (i) / 9
                    + s.start.X * (9 - i) / 9,
                s.finish.Y * (i) / 9 + s.start.Y * (9 - i) / 9), 1, grainAngle);
                Plant.Leaf l1 = new Plant.Leaf(10, s.dir, new Point(s.finish.X * (i) / 9
                    + s.start.X * (9 - i) / 9,
                s.finish.Y * (i) / 9 + s.start.Y * (9 - i) / 9), 1, -grainAngle);
                l.CreateLeaf();
                l1.CreateLeaf();
                GraphicsPath lPath = new GraphicsPath();
                GraphicsPath rPath = new GraphicsPath();
                lPath.AddClosedCurve(l.leftSide.ToArray());
                rPath.AddClosedCurve(l.rightSide.ToArray());
                g.FillPath(new SolidBrush(palette[5]), lPath);
                g.FillPath(new SolidBrush(palette[4]), rPath);
                GraphicsPath l1Path = new GraphicsPath();
                GraphicsPath r1Path = new GraphicsPath();
                l1Path.AddClosedCurve(l1.leftSide.ToArray());
                r1Path.AddClosedCurve(l1.rightSide.ToArray());
                g.FillPath(new SolidBrush(palette[4]), l1Path);
                g.FillPath(new SolidBrush(palette[5]), r1Path);
            }

            g.DrawLine(new Pen(palette[4], 2f), s.start, s.finish);
        }

        private void DrawLeaf(int typeLeaf, Vector dir,
            int len, Plant.Segment s, Graphics g, bool withSegment, Plant p,
            int grainAngle, List<Color> palette)
        {
            Plant.Leaf l;
            if (withSegment)
            {
                Plant.Segment segm = p.CreateSegment(s, 0.5f, 0);
                g.DrawLine(new Pen(Color.Green, 1f), segm.start,
                    segm.finish);
                g.DrawLine(new Pen(Color.DarkSeaGreen, 1f), segm.start, segm.finish);
                l = new Plant.Leaf(len, -segm.dir, segm.finish, typeLeaf, 0);
            }
            else if (grainAngle == 0) l = new Plant.Leaf(len, new Vector(dir.Y, dir.X),
                new Point((int)(s.start.X +
                 rnd.NextDouble() * (s.finish.X - s.start.X)), (int)(s.start.Y +
                 rnd.NextDouble() * (s.finish.Y - s.start.Y))), typeLeaf, 0);
            else l = new Plant.Leaf(len, new Vector(dir.Y, dir.X), new Point((int)(s.start.X +
                rnd.NextDouble() * (s.finish.X - s.start.X)), (int)(s.start.Y +
                rnd.NextDouble() * (s.finish.Y - s.start.Y))), typeLeaf, grainAngle);
            l.CreateLeaf();
            //g.DrawPolygon(new Pen(Color.Aqua, 2.0f), l.leftSide.ToArray());
            //g.DrawCurve(new Pen(Color.Aqua, 2.0f), l.rightSide.ToArray());
            GraphicsPath lPath = new GraphicsPath();
            GraphicsPath rPath = new GraphicsPath();
            lPath.AddClosedCurve(l.leftSide.ToArray());
            rPath.AddClosedCurve(l.rightSide.ToArray());
            g.FillPath(new SolidBrush(palette[2]), lPath);
            g.FillPath(new SolidBrush(palette[3]), rPath);
        }

        private void DrawBerry(Point start, Plant p, Plant.Segment s,
            Graphics g, List<Color> palette, float sAngle, float eAngle, bool withSegment)
        {
            Plant.Berry berry1;
            if (withSegment)
            {
                Plant.Segment segm = p.CreateSegment(s, 0.5f, 0);
                g.DrawLine(new Pen(Color.Black, 1f), segm.start,
                    new Point(2 * segm.start.X - segm.finish.X, 2 * segm.start.Y - segm.finish.Y));
                g.DrawLine(new Pen(Color.Brown, 1f), segm.start,
                    new Point(2 * segm.start.X - segm.finish.X + 1,
                    2 * segm.start.Y - segm.finish.Y));
                berry1 = new Plant.Berry(rnd.Next(5, 15),
                    new Point(2 * segm.start.X - segm.finish.X, 2 * segm.start.Y - segm.finish.Y),
                    sAngle, eAngle);
            }
            else berry1 = new Plant.Berry(rnd.Next(5, 15), s.start, sAngle, eAngle);
            GraphicsPath berry = new GraphicsPath();
            berry.AddEllipse(new Rectangle(berry1.smallP.X, berry1.smallP.Y,
                berry1.sRadius * 2, berry1.sRadius * 2));
            g.FillPath(new SolidBrush(palette[4]), berry);
            GraphicsPath berryShadow = new GraphicsPath();
            berryShadow.FillMode = FillMode.Alternate;
            berryShadow.AddArc(new Rectangle(berry1.smallP.X, berry1.smallP.Y,
               berry1.sRadius * 2, berry1.sRadius * 2),
               sAngle, eAngle);
            berryShadow.AddArc(new Rectangle(berry1.bigP.X, berry1.bigP.Y,
                berry1.bRadius * 2, berry1.bRadius * 2),
                berry1.sbAngle * 180 / (float)Math.PI,
                (berry1.ebAngle - berry1.sbAngle) * 180 / (float)Math.PI);
            g.FillPath(new SolidBrush(palette[5]), berryShadow);
            //g.DrawArc(new Pen(Color.Brown, 2f), new Rectangle(berry1.smallP.X, berry1.smallP.Y,
            //    berry1.sRadius * 2, berry1.sRadius * 2),
            //    sAngle, eAngle);
            //g.DrawArc(new Pen(Color.Black, 2f), new Rectangle(berry1.bigP.X, berry1.bigP.Y,
            //    berry1.bRadius * 2, berry1.bRadius * 2),
            //    berry1.sbAngle * 180 / (float)Math.PI,
            //(berry1.ebAngle - berry1.sbAngle) * 180 / (float)Math.PI);
            //g.DrawLine(new Pen(Color.BlanchedAlmond, 2f),
            //new Point(berry1.bigCenter.X - berry1.bRadius,
            //    berry1.bigCenter.Y - berry1.bRadius),
            //    berry1.bigCenter);
            //g.DrawLine(new Pen(Color.DarkMagenta, 2f), berry1.left, berry1.right);
            //g.DrawLine(new Pen(Color.BlueViolet, 2f), berry1.center, berry1.smallP);

        }

        private void DrawPlant(int X)
        {
            Graphics g = pictureBox1.CreateGraphics();
            g.SmoothingMode = SmoothingMode.HighQuality;
            Plant pl1 = new Plant(10, 5, new Point(X, 300), rnd.Next(0, 2), false, rnd.Next(0, 3));
            List<Color> palette = CreateColors(pl1.plantType);
            for (int i = 0; i < palette.Count; i++)
            {
                palette[i] = RandomizeColor(palette[i], 10);
            }
            Pen plantPen = new Pen(palette[2], 2.0f);
            Pen shadowPen = new Pen(palette[3], 2.0f);
            g.DrawCurve(plantPen, new Point[] { new Point(0, 100), new Point(0, 100) });
            pl1.CreatePlant();
            int typeLeaf = rnd.Next(0, 2);
            bool isStraight = rnd.Next(0, 2) == 1 ? true : false;
            bool withSegmentBerry = rnd.Next(0, 2) == 1 ? true : false;
            float sAngle = (float)(-60 + rnd.NextDouble() * 60);
            float eAngle = (float)(rnd.NextDouble() * 100 + 150);
            for (int i = 0; i < pl1.lineArr.Count; i++)
            {
                for (int j = 0; j < pl1.lineArr[i].Count(); j++)
                {
                    if (isStraight)
                    {
                        g.DrawLine(shadowPen, pl1.lineArr[i][j].start, pl1.lineArr[i][j].finish);
                        g.DrawLine(plantPen, pl1.lineArrShadow[i][j].start,
                            pl1.lineArrShadow[i][j].finish);
                    }
                    if (rnd.Next(0, 3) == 2 && (i != 0 || j != 0))
                    {
                        // Draw leaf.
                        // Place on the segment.
                        double place = rnd.NextDouble();
                        DrawLeaf(typeLeaf, pl1.lineArr[i][j].dir, pl1.lineArr[i][j].length,
                            pl1.lineArr[i][j], g,
                            rnd.Next(0, 2) == 1 ? true : false, pl1, 0, palette);
                    }
                    if (rnd.Next(0, 2) == 1 && (i != 0 || j != 0) && pl1.plantType == 2)
                    {
                        double place = rnd.NextDouble();
                        DrawBerry(new Point((int)(pl1.lineArr[i][j].start.X +
                            place * (pl1.lineArr[i][j].finish.X - pl1.lineArr[i][j].start.X)),
                            (int)(pl1.lineArr[i][j].start.Y +
                            place * (pl1.lineArr[i][j].finish.Y - pl1.lineArr[i][j].start.Y))),
                            pl1, pl1.lineArr[i][j], g, palette,
                            sAngle, eAngle, withSegmentBerry);
                    }
                    if (j == pl1.lineArr[i].Count - 1)
                    {
                        if (pl1.plantType == 0)
                            DrawFlower(g, pl1.lineArr[i][j], pl1, pl1.fl, palette);
                        else if (pl1.plantType == 1)
                            DrawGrain(g, pl1.lineArr[i][j], pl1, palette);
                    }
                }
            }
            // DrawCurve.
            if (!isStraight)
            {
                for (int i = 0; i < pl1.lineArr.Count; i++)
                {
                    Point[] pointsForCurve = new Point[pl1.lineArr[i].Count + 1];
                    Point[] pointsForCurveShadow = new Point[pl1.lineArrShadow[i].Count + 1];
                    for (int j = 0; j < pl1.lineArr[i].Count(); j++)
                    {
                        pointsForCurve[j] = pl1.lineArr[i][j].start;
                        pointsForCurveShadow[j] = pl1.lineArrShadow[i][j].start;
                    }
                    pointsForCurve[pointsForCurve.Length - 1] = pl1.lineArr[i].Last().finish;
                    pointsForCurveShadow[pointsForCurve.Length - 1]
                        = pl1.lineArrShadow[i].Last().finish;
                    g.DrawCurve(shadowPen, pointsForCurveShadow.ToArray());
                    g.DrawCurve(plantPen, pointsForCurve.ToArray());
                }
            }
        }

        public List<Color> CreateColors(int plType)
        {
            List<Color> res = new List<Color>();
            int num = rnd.Next(0, segmentLightColors.Count());
            res.Add(segmentLightColors[num]);
            res.Add(segmentShadowColors[num]);
            num = rnd.Next(0, leafLightColors.Count());
            res.Add(leafLightColors[num]);
            res.Add(leafShadowColors[num]);
            // Flower.
            if (plType == 0)
            {
                num = rnd.Next(0, flowerPetal1Colors.Count());
                res.Add(flowerPetal1Colors[num]);
                res.Add(flowerPetal2Colors[num]);
                if (rnd.Next(0, 2) == 1)
                {
                    res.Reverse(res.Count() - 2, 2);
                }
                num = rnd.Next(0, flowerCenter1Colors.Count());
                res.Add(flowerCenter1Colors[num]);
                res.Add(flowerCenter2Colors[num]);
                if (rnd.Next(0, 2) == 1)
                {
                    res.Reverse(res.Count() - 2, 2);
                }
            }
            else if (plType == 1)
            {
                num = rnd.Next(0, grainColors.Count());
                res.Add(grainColors[num]);
                num = rnd.Next(0, grainColors.Count());
                res.Add(grainColors[num]);
            }
            else
            {
                num = rnd.Next(0, berryLightColors.Count());
                res.Add(berryLightColors[num]);
                res.Add(berryShadowColors[num]);
            }
            return res;
        }

        private Color RandomizeColor(Color c, int offset)
        {
            int[] rgb = new int[3];
            int[] rgbRef = new int[3] { c.R, c.G, c.B };
            for (int i = 0; i < 3; i++)
            {
                rgb[i] = rgbRef[i] + rnd.Next(-offset, offset + 1);
                if (rgb[i] > 255)
                    rgb[i] = 255;
                else if (rgb[i] < 0)
                    rgb[i] = 0;
            }
            return Color.FromArgb(255, rgb[0], rgb[1], rgb[2]);
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            pictureBox1.Image = null;
            Xoffset = 60;
            MouseEventArgs me = (MouseEventArgs)e;
            Point coordinates = me.Location;
            label1.Text = coordinates.Y.ToString();
        }


        private void bCreatePlant_Click(object sender, EventArgs e)
        {
            DrawPlant(Xoffset);

            Xoffset += 100;
        }

        private void bCreateLandscape_Click(object sender, EventArgs e)
        {
            DelaunayTriangulator delaunay = new DelaunayTriangulator();
            Points = delaunay.GeneratePoints(PointCount, DiagramWidth, DiagramHeight).ToList();

            var delaunayTimer = Stopwatch.StartNew();
            trList = delaunay.BowyerWatson(Points).ToList();
            delaunayTimer.Stop();

            Graphics g = pictureBox1.CreateGraphics();

            for (int i = 0; i < trList.Count; i++)
                trList[i].SortVertices();
            CreateDependencies();
            CreateTriangleSequence(0);

            DrawTriangulation(trList, g);
            AddHeight();
            CreatePerspective(10);
            CreatePaintPlains();
            PaintPlains(g);
            SortTriangles();
            numForSequence = 0;
        }

        private void SortTriangles()
        {
            Triangle[] newList = new Triangle[trList.Count];
            for (int i = 0; i < trList.Count; i++)
            {
                newList[trList[i].seq] = PerspectiveTriangles[i];
            }
            PerspectiveTriangles = newList.ToList();
        }

        private void PaintPerspective(Graphics g)
        {
            for (int i = 0; i < trList.Count(); i++)
            {
                SolidBrush b = new SolidBrush(trList[i].clr);
                PointF[] toPaint = new PointF[3] { new PointF((float)PerspectiveTriangles[i].Vertices[0].X,
                    (float)PerspectiveTriangles[i].Vertices[0].Y),
                new PointF((float)PerspectiveTriangles[i].Vertices[1].X,
                    (float)PerspectiveTriangles[i].Vertices[1].Y),
                new PointF((float)PerspectiveTriangles[i].Vertices[2].X,
                    (float)PerspectiveTriangles[i].Vertices[2].Y)};
                //if (i == 1)
                g.FillPolygon(b, toPaint);
            }
        }

        /// <summary>
        /// Creating triangle dependencies and obstacles.
        /// </summary>
        /// 
        private void CreateDependencies()
        {
            for (int i = 0; i < trList.Count; i++)
            {
                if (trList[i].isOneSide)
                {
                    for (int j = 0; j < trList.Count; j++)
                        if (i != j && ((trList[i].Vertices[0] == trList[j].Vertices[0])
                            || (trList[i].Vertices[0] == trList[j].Vertices[1])
                            || (trList[i].Vertices[0] == trList[j].Vertices[2])))
                            if ((trList[i].Vertices[2] == trList[j].Vertices[0])
                                || (trList[i].Vertices[2] == trList[j].Vertices[1])
                                || (trList[i].Vertices[2] == trList[j].Vertices[2]))
                            {
                                trList[i].depInd = j;
                                if (trList[j].obsInd == -1)
                                    trList[j].obsInd = i;
                                else trList[j].depOrObs = i;
                                break;
                            }
                }
                else
                {
                    for (int j = 0; j < trList.Count; j++)
                    {
                        if (i != j && ((trList[i].Vertices[1] == trList[j].Vertices[0])
                            || (trList[i].Vertices[1] == trList[j].Vertices[1])
                            || (trList[i].Vertices[1] == trList[j].Vertices[2])))
                            if ((trList[i].Vertices[0] == trList[j].Vertices[0])
                                || (trList[i].Vertices[0] == trList[j].Vertices[1])
                                || (trList[i].Vertices[0] == trList[j].Vertices[2]))
                            {
                                if (trList[j].obsInd == -1)
                                    trList[j].obsInd = i;
                                else trList[j].depOrObs = i;
                                if (trList[i].depInd == -1)
                                    trList[i].depInd = j;
                                else { trList[i].depOrObs = j; break; }
                            }
                            else if ((trList[i].Vertices[2] == trList[j].Vertices[0])
                                || (trList[i].Vertices[2] == trList[j].Vertices[1])
                                || (trList[i].Vertices[2] == trList[j].Vertices[2]))
                            {
                                if (trList[j].obsInd == -1)
                                    trList[j].obsInd = i;
                                else trList[j].depOrObs = i;
                                if (trList[i].depInd == -1)
                                    trList[i].depInd = j;
                                else { trList[i].depOrObs = j; break; }
                            }
                    }
                }
            }
        }

        /// <summary>
        /// Create painting order in triangles structure recursively (reccurently :).
        /// </summary>
        /// <param name="num">number in sequence.</param>
        /// <param name="ind">index of observed triangle in list.</param>
        private void CreateTriangleSequence(int ind)
        {
            //if (trList[ind].seq != -1 || numForSequence == trList.Count())
            //  return;

            if (trList[ind].isOneSide)
            {
                // Check dependency.
                if (trList[ind].depInd != -1 &&
                    trList[trList[ind].depInd].seq == -1)
                {
                    CreateTriangleSequence(trList[ind].depInd);
                    return;
                }
                // If dependency in sequence, then go to obstacle
                // and increment numForSequence.
                else
                {
                    if (trList[ind].seq == -1)
                    {
                        trList[ind].seq = numForSequence;
                        numForSequence++;
                    }
                    if (trList[ind].obsInd != -1)
                        CreateTriangleSequence(trList[ind].obsInd);
                    if (trList[ind].depOrObs != -1)
                        CreateTriangleSequence(trList[ind].depOrObs);
                }
            }
            else
            {
                // Check dependencies.
                if (trList[ind].depInd != -1 &&
                    trList[trList[ind].depInd].seq == -1)
                {
                    CreateTriangleSequence(trList[ind].depInd);
                    return;
                }
                else if (trList[ind].depOrObs != -1 &&
                    trList[trList[ind].depOrObs].seq == -1)
                {
                    CreateTriangleSequence(trList[ind].depOrObs);
                    return;
                }
                else
                {
                    if (trList[ind].seq == -1)
                    {
                        trList[ind].seq = numForSequence;
                        numForSequence++;
                    }
                    if (trList[ind].obsInd != -1)
                        CreateTriangleSequence(trList[ind].obsInd);
                }
            }
        }

        private void DrawTriangulation(IEnumerable<Triangle> triangulation, Graphics g)
        {
            Pen dPen = new Pen(Color.Black, 1f);

            g.Clear(Color.White);
            var edges = new List<Edge>();
            for (int i = 0; i < trList.Count; i++)
            {
                var triangle = trList[i];
                edges.Add(new Edge(triangle.Vertices[0], triangle.Vertices[1]));
                edges.Add(new Edge(triangle.Vertices[1], triangle.Vertices[2]));
                edges.Add(new Edge(triangle.Vertices[2], triangle.Vertices[0]));
                g.DrawString(i + " " + (triangle.isOneSide ? "1 " : "2 ") + "\n" +
                    triangle.depInd + " " + triangle.depOrObs + " "
                    + triangle.obsInd + "\n" + triangle.seq
                    , new Font(System.Drawing.FontFamily.GenericSansSerif,
            12.0F, System.Drawing.FontStyle.Bold), new SolidBrush(Color.BlueViolet),
            new Point((int)(triangle.Vertices[0].X + triangle.Vertices[1].X + triangle.Vertices[2].X) / 3
            , (int)(triangle.Vertices[0].Y + triangle.Vertices[1].Y + triangle.Vertices[2].Y) / 3));
            }

            foreach (var edge in edges)
            {

                Point strt = new Point((int)edge.Point1.X, (int)edge.Point1.Y);
                Point fnsh = new Point((int)edge.Point2.X, (int)edge.Point2.Y);

                g.DrawLine(dPen, strt, fnsh);
            }
        }

        public void AddHeight()
        {
            for (int i = 0; i < Points.Count(); i++)
            {
                // rnd.Next(0,50)
                Points[i].Z = rnd.Next(0, 50);
            }

        }

        public void CreatePaintPlains()
        {
            for (int i = 0; i < trList.Count(); i++)
            {
                Vector3D v1 = new Vector3D
                    ((trList[i].Vertices[0].Y - trList[i].Vertices[1].Y) *
                    (trList[i].Vertices[0].Z - trList[i].Vertices[2].Z) -
                    (trList[i].Vertices[0].Y - trList[i].Vertices[2].Y) *
                    (trList[i].Vertices[0].Z - trList[i].Vertices[1].Z),
                    -(trList[i].Vertices[0].X - trList[i].Vertices[1].X) *
                    (trList[i].Vertices[0].Z - trList[i].Vertices[2].Z) +
                    (trList[i].Vertices[0].X - trList[i].Vertices[2].X) *
                    (trList[i].Vertices[0].Z - trList[i].Vertices[1].Z),
                    (trList[i].Vertices[0].X - trList[i].Vertices[1].X) *
                    (trList[i].Vertices[0].Y - trList[i].Vertices[2].Y) -
                    (trList[i].Vertices[0].X - trList[i].Vertices[2].X) *
                    (trList[i].Vertices[0].Y - trList[i].Vertices[1].Y));
                Vector3D v = new Vector3D(1, 0, 1);
                double res;
                if (Vector3D.AngleBetween(v, v1) > 90)
                    res = (Vector3D.AngleBetween(v, v1) - 90) / 180 * Math.PI;
                else res = Vector3D.AngleBetween(v, v1) / 180 * Math.PI;
                Color src = Color.DarkSeaGreen;
                double lght = src.GetBrightness() * Math.Cos(res);
                if (i == 1)
                    label1.Text = Vector3D.AngleBetween(v, v1).ToString();
                trList[i].clr = FromHSL(src.GetHue() / 360, src.GetSaturation(), (float)lght);
                PerspectiveTriangles[i].clr = FromHSL(src.GetHue() / 360, src.GetSaturation(), (float)lght);

            }
        }

        public void PaintPlains(Graphics g)
        {
            for (int i = 0; i < trList.Count(); i++)
            {
                SolidBrush b = new SolidBrush(trList[i].clr);
                PointF[] toPaint = new PointF[3] { new PointF((float)trList[i].Vertices[0].X,
                    (float)trList[i].Vertices[0].Y),
                new PointF((float)trList[i].Vertices[1].X,
                    (float)trList[i].Vertices[1].Y),
                new PointF((float)trList[i].Vertices[2].X,
                    (float)trList[i].Vertices[2].Y)};
                //if (i == 1)
                g.FillPolygon(b, toPaint);
            }
        }

        public Color FromHSL(float h, float s, float l)
        {
            float r, g, b;

            if (s == 0f)
            {
                r = g = b = l; // achromatic
            }
            else
            {
                float q = l < 0.5f ? l * (1 + s) : l + s - l * s;
                float p = 2 * l - q;
                r = hueToRgb(p, q, h + 1f / 3f);
                g = hueToRgb(p, q, h);
                b = hueToRgb(p, q, h - 1f / 3f);
            }
            return Color.FromArgb(255, to255(r), to255(g), to255(b));
        }

        public float hueToRgb(float p, float q, float t)
        {
            if (t < 0f)
                t += 1f;
            if (t > 1f)
                t -= 1f;
            if (t < 1f / 6f)
                return p + (q - p) * 6f * t;
            if (t < 1f / 2f)
                return q;
            if (t < 2f / 3f)
                return p + (q - p) * (2f / 3f - t) * 6f;
            return p;
        }

        public int to255(float v) { return (int)Math.Min(255, 256 * v); }

        public void CreatePerspective(double angle)
        {
            PerspectivePoints = new List<lPoint>();
            PerspectiveTriangles = new List<Triangle>();
            double a = Math.Cos(angle / 180 * Math.PI);
            double b = Math.Sin(angle / 180 * Math.PI);
            for (int i = 0; i < Points.Count; i++)
            {
                int yp = (int)Points[i].Y;
                int zp = Points[i].Z;
                double t = (b * zp - a * yp) / (a * a + b * b);
                PerspectivePoints.Add(new lPoint(Points[i].X,
                    Math.Sqrt(Math.Pow(yp + a * t, 2) + Math.Pow(zp - b * t, 2))));
            }
            for (int i = 0; i < trList.Count(); i++)
            {
                PerspectiveTriangles.Add(new Triangle());
                for (int j = 0; j < 3; j++)
                {
                    int yp = (int)trList[i].Vertices[j].Y;
                    int zp = trList[i].Vertices[j].Z;
                    double t = (b * zp - a * yp) / (a * a + b * b);
                    PerspectiveTriangles[i].Vertices[j] = new lPoint(trList[i].Vertices[j].X,
                    Math.Sqrt(Math.Pow(yp + a * t, 2) + Math.Pow(zp - b * t, 2)));
                }
            }
        }

        private void bCreatePerspective_Click(object sender, EventArgs e)
        {
            Graphics g = pictureBox1.CreateGraphics();
            g.Clear(Color.White);
            CreatePerspective(20);
            //PointF[] pArr = new PointF[PerspectivePoints.Count];
            //for (int i = 0; i < PerspectivePoints.Count; i++)
            //{
            //    pArr[i] = new PointF((float)PerspectivePoints[i].X, (float)PerspectivePoints[i].Y);
            //}
            //g.DrawPolygon(new Pen(Color.Black, 2f), pArr);


            // For drawing triangulation edges.

            //for (int i = 0; i < PerspectiveTriangles.Count; i++)
            //{
            //    PointF[] arr = new PointF[3];
            //    for (int j = 0; j < 3; j++)
            //    {
            //        arr[j] = new PointF((float)PerspectiveTriangles[i].Vertices[j].X,
            //            (float)PerspectiveTriangles[i].Vertices[j].Y);
            //    }
            //    g.DrawPolygon(new Pen(Color.Black, 2f), arr);
            //}
            PaintPerspective(g);
        }
    }
    class Plant
    {
        bool withBerry;
        static Random rnd = new Random();
        public int height;
        int numSegments;
        public List<List<Segment>> lineArr;
        public List<List<Segment>> lineArrShadow;
        public Point root;
        public int stem;
        public Leaf leaf;
        public Flower fl;
        public int plantType;

        public Plant(int h, int n, Point r, int s, bool w, int plType)
        {
            lineArr = new List<List<Segment>>();
            lineArrShadow = new List<List<Segment>>();
            height = h;
            numSegments = n;
            root = r;
            stem = s;
            withBerry = w;
            plantType = plType;
        }

        public Segment CreateSegment(Segment prev, float len, int forWhat)
        {
            if (forWhat == 0)
            {
                double a = rnd.NextDouble() * 0.5 * (rnd.Next(0, 2) == 0 ? 1 : -1);
                // What segment will be on what side.
                int leftOrRight = -1;
                if (rnd.Next(0, 2) == 1)
                    leftOrRight = 1;
                double b = rnd.NextDouble() * 0.5 * (rnd.Next(0, 2) == 0 ? 1 : -1) + 0.1;
                Segment segm1 = new Segment(prev.dir *
                        new System.Windows.Media.Matrix(Math.Cos(a + b * leftOrRight),
                        -Math.Sin(a + b * leftOrRight),
                        Math.Sin(a + b * leftOrRight),
                        Math.Cos(a + b * leftOrRight), 0, 0),
                        prev.length,
                        prev.finish,
                        prev.segmNum + 1);
                segm1.finish = new Point(Convert.ToInt32(
                    (segm1.start.X + len * (segm1.dir * 15).X)),
                   Convert.ToInt32((segm1.start.Y + len * (segm1.dir * 15).Y)));
                return segm1;
            }
            else return new Segment(new Vector(1, 1), 1, new Point(10, 10), 0);
        }




        public void CreatePlant()
        {
            fl = new Flower();
            int whatArr = 0;
            lineArr.Add(new List<Segment>());
            lineArr[0].Add(new Segment(new Vector(0, -2), 10, root, 0));
            lineArr[0][0].finish = new Point(root.X, root.Y - 10);
            lineArrShadow.Add(new List<Segment>());
            lineArrShadow[0].Add(new Segment(new Vector(0, -2), 10,
                new Point(root.X + 2, root.Y), 0));
            lineArrShadow[0][0].finish = new Point(root.X + 2, root.Y - 10);
            while (whatArr < lineArr.Count && lineArr.Count < 5)
            {
                double chance = rnd.NextDouble();
                //double a = rnd.NextDouble() * 0.5 * (rnd.Next(0, 2) == 0 ? 1 : -1);
                if (lineArr[whatArr].Count == numSegments)
                {
                    whatArr++;
                    continue;
                    // Chance to leave while before the end.

                    //if (rnd.NextDouble() > 0.7)
                    //    break;
                }
                if (chance > 10.9)
                {
                    // Create three.
                }
                else if (chance > 0.7)
                {
                    // Create two.
                    lineArr.Add(new List<Segment>());
                    lineArrShadow.Add(new List<Segment>());
                    lineArr[whatArr].Add(CreateSegment(lineArr[whatArr].Last(), 1, 0));
                    Segment segm1Shadow = new Segment(lineArr[whatArr].Last().dir,
                        lineArr[whatArr].Last().length,
                        new Point(lineArr[whatArr].Last().start.X + 2,
                        lineArr[whatArr].Last().start.Y), lineArr[whatArr].Last().segmNum);
                    segm1Shadow.finish = new Point(lineArr[whatArr].Last().finish.X + 2,
                        lineArr[whatArr].Last().finish.Y);
                    lineArrShadow[whatArr].Add(segm1Shadow);

                    lineArr.Last().Add(CreateSegment
                        (lineArr[whatArr][lineArr[whatArr].Count - 2], 1, 0));
                    Segment segm2Shadow = new Segment(lineArr.Last().Last().dir,
                        lineArr.Last().Last().length,
                        new Point(lineArr.Last().Last().start.X + 2,
                        lineArr.Last().Last().start.Y),
                        lineArr.Last().Last().segmNum);
                    segm2Shadow.finish = new Point(lineArr.Last().Last().finish.X + 2,
                        lineArr.Last().Last().finish.Y);
                    lineArrShadow.Last().Add(segm2Shadow);

                }
                else
                {
                    // Create one.

                    lineArr[whatArr].Add(CreateSegment(lineArr[whatArr].Last(), 1, 0));
                    Segment segmShadow = new Segment(lineArr[whatArr].Last().dir,
                        lineArr[whatArr].Last().length,
                        new Point(lineArr[whatArr].Last().start.X + 2,
                        lineArr[whatArr].Last().start.Y),
                        lineArr[whatArr].Last().segmNum);
                    segmShadow.finish = new Point(lineArr[whatArr].Last().finish.X + 2,
                        lineArr[whatArr].Last().finish.Y);
                    lineArrShadow[whatArr].Add(segmShadow);
                }
            }
        }

        public class Flower
        {

            public int width;
            public int sWidth;
            public int petalsNum;

            public Flower()
            {
                width = rnd.Next(10, 50);
                petalsNum = rnd.Next(3, 7);
                sWidth = width - rnd.Next(5, (int)(width * 3 / 4) + 1);
            }
        }

        public class Berry
        {
            public int sRadius;
            public int bRadius;
            public Point smallP;
            public Point bigP;
            public Point center;
            public Point left;
            public Point right;
            public Point bigCenter;
            public float sbAngle;
            public float ebAngle;

            public Berry(int size, Point c, float sAngle, float eAngle)
            {
                sRadius = size / 2;
                center = c;
                smallP = new Point(center.X - sRadius, center.Y - sRadius);
                left = new Point(Convert.ToInt32(center.X + Math.Cos(sAngle * Math.PI / 180) * sRadius),
                    Convert.ToInt32(center.Y + Math.Sin(sAngle * Math.PI / 180) * sRadius));
                right = new Point(Convert.ToInt32(center.X +
                    Math.Cos((sAngle + eAngle) * Math.PI / 180) * sRadius),
                    Convert.ToInt32(center.Y + Math.Sin((sAngle + eAngle) * Math.PI / 180) * sRadius));
                Point mid = new Point((left.X + right.X) / 2, (left.Y + right.Y) / 2);
                if (eAngle < 180)
                {
                    bigCenter = new Point((center.X + 3 * (center.X - mid.X)),
                        (center.Y + 3 * (center.Y - mid.Y)));
                }
                else bigCenter = new Point((center.X + 3 * (-center.X + mid.X)),
                       (center.Y + 3 * (-center.Y + mid.Y)));
                bRadius = Convert.ToInt32(Math.Sqrt(Math.Pow(right.X - bigCenter.X, 2)
                    + Math.Pow(right.Y - bigCenter.Y, 2)));
                bigP = new Point(bigCenter.X - bRadius, bigCenter.Y - bRadius);
                sbAngle = (float)Math.Atan2(Convert.ToSingle(left.Y - bigCenter.Y),
                    Convert.ToSingle(left.X - bigCenter.X));
                ebAngle = (float)Math.Atan2(Convert.ToSingle(right.Y - bigCenter.Y),
                    Convert.ToSingle(right.X - bigCenter.X));
            }
        }

        public class Leaf
        {
            public int length;
            public Vector dir;
            public Point start;
            public List<Point> leftSide = new List<Point>();
            public List<Point> rightSide = new List<Point>();
            public int leafType;
            public int grainAngle;

            public Leaf(int l, Vector d, Point p, int t, int gA)
            {
                length = l;
                dir = d;
                start = p;
                leafType = t;
                grainAngle = gA;
            }

            public void CreateLeaf()
            {
                double a = rnd.NextDouble() * 0.5 * (rnd.Next(0, 2) == 0 ? 1 : -1);
                int coef;
                int c2 = 10;
                if (leafType == 0)
                {
                    coef = rnd.Next(5, 15);
                    c2 = 2;
                }
                else coef = rnd.Next(10, 25);
                if (grainAngle != 0)
                {
                    coef = 10;
                    c2 = 7;
                    dir = dir * new System.Windows.Media.Matrix
                        (Math.Cos(grainAngle * Math.PI / 180),
                        -Math.Sin(grainAngle * Math.PI / 180),
                    Math.Sin(grainAngle * Math.PI / 180),
                    Math.Cos(grainAngle * Math.PI / 180), 0,
                         rnd.Next(0, 2) == 0 ? 1 : -1);
                }
                else
                    dir = dir * new System.Windows.Media.Matrix(Math.Cos(a), -Math.Sin(a),
                    Math.Sin(a), Math.Cos(a), 0, 0) * (rnd.Next(0, 2) == 0 ? 1 : -1);
                leftSide.Add(start);
                leftSide.Add(new Point(
                    (int)(leftSide[0].X + dir.X * coef),
                    (int)(leftSide[0].Y + dir.Y * coef)));
                Point mid = new Point((start.X + leftSide[1].X) / 2,
                    (start.Y + leftSide[1].Y) / 2);
                rightSide.Add(leftSide[0]);
                rightSide.Add(leftSide[1]);
                leftSide.Add(new Point((int)(dir.Y * coef / c2 + rnd.Next(-20, 20) / c2 + mid.X),
                    (int)(dir.X * coef / c2 + rnd.Next(-20, 20) / c2 + mid.Y)));
                rightSide.Add(new Point((int)(-dir.Y * coef / c2 + rnd.Next(-20, 20) / c2 + mid.X),
                    (int)(dir.X * coef / c2 + rnd.Next(-20, 20) / c2 + mid.Y)));
                //leftSide.Insert(2, new Point(leftSide[1].X / 2 + leftSide[2].X / 2,
                //leftSide[1].Y / 2 + leftSide[2].Y / 2));
            }
        }

        public class Segment
        {
            public int length;
            public Vector dir;
            public int segmNum;
            public Point start;
            public Point finish;
            bool taken;

            public Segment(Vector d, int l, Point s, int segmN)
            {
                taken = false;
                dir = d;
                length = l;
                start = s;
                segmNum = segmN;
            }
        }
    }
}
