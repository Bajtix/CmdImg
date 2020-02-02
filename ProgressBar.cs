using System;

namespace dcitysim
{
    partial class Program
    {
        //Class that handles the functions
        public class ProgressBar
        {
            public class ProgressBarStyle
            { 
                public char open; 
                public char close; 
                public char progress;
                public char noprogress;
                public char tip;

                public ProgressBarStyle(char open, char close, char progress, char tip,char noprogress)
                {
                    this.open = open;
                    this.close = close;
                    this.progress = progress;
                    this.noprogress = noprogress;
                    this.tip = tip;
                }
            }
            //Variables
            public float progress;
            public ProgressBarStyle style;
            public int width;
            private string graphic;
            private int rp = 0;
            public ProgressBar(float progress, ProgressBarStyle style, int width)
            {
                this.progress = progress;
                this.style = style;
                this.width = width;
            }

            //Functions
            public void Generate()
            {
                string bar = style.open.ToString();
                int pr = (int)Math.Round(progress * width);
                int np = width - pr;
                for(int i = 0; i<width; i++)
                {
                    if(i<pr)
                        bar = bar + style.progress;
                    if (i == pr)
                        bar = bar + style.tip;
                    if(i>pr)
                        bar = bar + style.noprogress;
                }
                bar = bar + style.close + " " + Math.Floor(progress * 100) + "%";
                
                graphic = bar;
                
            }
            
            public void Draw()
            {
                rp = Console.CursorLeft;
                Console.Write(graphic);
                if (progress != 1)
                    Console.CursorLeft = rp;
                else
                    Console.Write(" ");
                
            }
        }
        
    }
}
