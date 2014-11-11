using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AI;

namespace TicTacToe
{
    public class TicTacToeTrasform: Transform
    {
        const int Size = 3;
        delegate Point TransformFunc(Point p);
        public static Point Rotate90Degree(Point p)
        {
            //012 -> x->y, y->size-x
            //012
            return new Point(Size - p.y -1, p.x);
        }
        public static Point MirrorX(Point p)
        {
            //012 -> 210
            return new Point(Size - p.x -1, p.y);
        }
        public static Point MirrorY(Point p)
        {
            return new Point(p.x, Size - p.y -1);
        }

        List<TransformFunc> actions = new List<TransformFunc>();
        public Point ActOn(Point p)
        {
            foreach (TransformFunc f in actions)
            {
                if(f!=null)
                    p = f(p);
            }

            return p;
        }

        TicTacToeTrasform(TransformFunc op, TransformFunc[] ops)
        {
            if(op!=null)
                actions.Add(op);
            if (ops!=null && ops.Length > 0)
                actions.AddRange(ops);
        }
        
        public static List<Transform> s_transforms = new List<Transform>();

        static TicTacToeTrasform()
        {
            for (int i = 0; i < 4; i++)
            {
                TransformFunc[] ops = Enumerable.Repeat<TransformFunc>(Rotate90Degree, i).ToArray();
                s_transforms.Add(new TicTacToeTrasform(null, ops));
                s_transforms.Add(new TicTacToeTrasform(MirrorX, ops));
                s_transforms.Add(new TicTacToeTrasform(MirrorY, ops));
            }
        }
    }
}
