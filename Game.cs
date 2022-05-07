using Aiv.Fast2D;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace boids
{
    static class Game
    {
        static public Window Window;
        static public List<Boid> Boids { get; private set; }
        static public Texture BoidTexture;

        static public float DeltaTime { get => Window.DeltaTime; }

        static float spawnCounter;
        static float spawnTimeToWait = 0.1f;

        static public TextObject[] texts;

        static float addD;
        static float addN;

        static public void Init()
        {
            Window = new Window(1280, 720, "Boid Project");
            Boids = new List<Boid>();
            Window.SetVSync(false);
            BoidTexture = new Texture("Assets/boid.png",true);

            Boid.SetWeigths();

            Font comic = new Font("Assets/comics.png", 10, 32, 61, 65);
            texts = new TextObject[4];
            texts[0] = new TextObject(new Vector2(20, 20), "Alignment: " + Boid.alignmentWeight/Boid.Delta, comic, -590);
            texts[1] = new TextObject(new Vector2(20, 40), "Cohesion: " + Boid.cohesionWeight/Boid.Delta, comic, -590);
            texts[2] = new TextObject(new Vector2(20, 60), "Separation: "+Boid.separationWeight/Boid.Delta, comic, -590);
            texts[3] = new TextObject(new Vector2(20, 80), "Vision Angle: "+Boid.HalfConeAngle*2, comic, -590);
        }

        static public void Play()
        {

            for (int i = 0; i < 1; i++)
            {
                Boids.Add(new Boid(new Vector2(400,350)));
            }
            Boids.Add(new Boid(new Vector2(400,350)));
            Boids.Add(new Boid(new Vector2(400,400)));
            Boids.Add(new Boid(new Vector2(400,400)));

            Boids[0].Velocity = new Vector2(-0.2f, -1);
            Boids[1].Velocity = new Vector2(-1, -1);


            while (Window.IsOpened)
            {
                Window.SetTitle($"Boids Project FPS: {1f / Window.DeltaTime}");

                // Exit when ESC is pressed
                if (Window.GetKey(KeyCode.Esc))
                {
                    break;
                }

                //INPUT

                Boid.ChangeWeights();

                if (Window.MouseLeft && spawnCounter < 0)
                {
                    Boids.Add(new Boid(Window.MousePosition));
                    spawnCounter = spawnTimeToWait;
                }

                if (DeltaTime != 0)
                {
                    addD += 0.0010f;
                    addN += DeltaTime;
                }

                //UPDATE

                foreach (Boid boid in Boids)
                {
                    boid.Update();
                }

                spawnCounter-=DeltaTime;

                //DRAW
                foreach (Boid boid in Boids)
                {
                    boid.Draw();
                }

                for (int i = 0; i < texts.Length; i++)
                {
                    texts[i].Draw();
                }

                Window.Update();
            }
        }        
    }
}
