using Aiv.Fast2D;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace boids
{
    class Boid
    {
        static public float Delta = 0.0005f;
        protected Sprite sprite { get; }
        protected Texture texture { get; }

        protected float flySpeed = 100;
        protected float HalfWidth;
        protected float HalfHeight;
        private static float halfConeAngle = MathHelper.DegreesToRadians(135);
        public static float HalfConeAngle { get => MathHelper.RadiansToDegrees(halfConeAngle); set => MathHelper.DegreesToRadians(value); }
        protected float AlignmentRay = 160;
        protected float CohesionRay = 130;
        protected float SeparationRay;
        public Vector2 Position { get => sprite.position; set => sprite.position = value; }
        public float X { get => sprite.position.X; set => sprite.position.X = value; }
        public float Y { get => sprite.position.Y; set => sprite.position.Y = value; }
        public Vector2 Forward
        {
            get => new Vector2((float)Math.Cos(sprite.Rotation), (float)Math.Sin(sprite.Rotation));
            set => sprite.Rotation = (float)Math.Atan2(value.Y, value.X);
        }

        public Vector2 Velocity = Vector2.Zero;

        static public float alignmentWeight, cohesionWeight, separationWeight;
        public Boid(Vector2 position)
        {   
            texture = Game.BoidTexture;
            HalfWidth = texture.Width * 0.5f;
            HalfHeight = texture.Height * 0.5f;

            sprite = new Sprite(HalfWidth * 1.5f, HalfHeight * 1.5f);
            sprite.pivot = new Vector2(HalfWidth, HalfHeight);

            SeparationRay = sprite.Width * 0.5f;

            Position = position;

            Velocity.X = RandomGenerator.GetRandomFloat() * flySpeed * RandomGenerator.GetRandomSign();
            Velocity.Y = RandomGenerator.GetRandomFloat() * flySpeed * RandomGenerator.GetRandomSign();

            sprite.SetAdditiveTint(RandomGenerator.GetRandomInt(-255,255), RandomGenerator.GetRandomInt(-255, 255), RandomGenerator.GetRandomInt(-255, 255), 0);
        }

        static public void SetWeigths()
        {
            alignmentWeight = 1 * Delta;
            cohesionWeight = 1 * Delta;
            separationWeight = 1 * Delta;
        }

        public void Update()
        {
            Vector2 alignment = GetAlignment(GetVisibleBoids(AlignmentRay));
            Vector2 cohesion = GetCohesion(GetVisibleBoids(CohesionRay));
            Vector2 separation = GetSeparation1(GetVisibleBoids(SeparationRay));

            
            //Movement
            Velocity = (Velocity + alignment * alignmentWeight + cohesion * cohesionWeight + separation * separationWeight*2).Normalized();
            Position += Velocity * flySpeed * Game.DeltaTime;

            Forward = Velocity;
            FixPosition();
        }

        public Vector2 GetCohesion(List<Boid> boids)
        {
            if (boids.Count < 1) return Vector2.Zero;

            //we have at least 1 boids

            Vector2 averagePosition = Vector2.Zero;

            for (int i = 0; i < boids.Count; i++)
            {
                averagePosition += boids[i].Position;
            }

            averagePosition/=boids.Count;

            return (averagePosition - Position).Normalized();
        }
        public Vector2 GetSeparation1(List<Boid> boids)
        {
            if (boids.Count < 1) return Vector2.Zero;

            //we have at least 1 boids
            Vector2 averagePosition = Vector2.Zero;

            for (int i = 0; i < boids.Count; i++)
            {
                averagePosition += (Position - boids[i].Position);
            }

            averagePosition /= boids.Count;
            averagePosition.Normalize();

            return averagePosition;
        }
        public Vector2 GetSeparation2(List<Boid> boids)
        {
            if (boids.Count < 1) return Vector2.Zero;

            //we have at least 1 boids
            Vector2 averagePosition = Vector2.Zero;

            // add the contribution of each neighbor towards me
            foreach (Boid other in boids)
            {
                Vector2 towardsMe = new Vector2();
                towardsMe = Position -  other.Position;

                // force contribution will vary inversely proportional
                if (towardsMe.Length > 0)
                {

                    // to distance or even the square of the distance
                    averagePosition += towardsMe.Normalized() /  towardsMe.Length;
                }
            }

            // return normalized vector
            return averagePosition.Normalized();
        }
        public Vector2 GetSeparation3(List<Boid> Neighbors)
        {
            if (Neighbors.Count == 0) return Vector2.Zero;
            float minDistance = 2000;
            float avoidFactor = 6f;
            Vector2 distance = Vector2.Zero;

            for (int i = 0; i < Neighbors.Count; i++)
            {
                var vector2 = Neighbors[i].Position - Position;

                if (vector2.LengthSquared < minDistance)
                {
                    distance -= vector2;
                }
            }
            if (distance == Vector2.Zero) return Vector2.Zero;

            return distance.Normalized() * avoidFactor;
        }
        public Vector2 GetAlignment(List<Boid> boids)
        {
            if (boids.Count < 1) return Vector2.Zero;

            //we have at least 1 boids

            Vector2 averageForward = Vector2.Zero;

            for (int i = 0; i < boids.Count; i++)
            {
                averageForward += boids[i].Velocity;
            }

            averageForward /= boids.Count; //serve?

            averageForward.Normalize();

            return averageForward;
        }
        public List<Boid> GetVisibleBoids(float ray)
        {
            List<Boid> boids = Game.Boids;
            List<Boid> visibleBoids = new List<Boid>();

            for (int i = 0; i < boids.Count; i++)
            {
                if (boids[i] == this) continue;

                Vector2 dist = boids[i].Position - Position;

                float raySquared = ray * ray;

                if (dist.LengthSquared < raySquared)
                {
                    float angleCos = Vector2.Dot(Forward, dist.Normalized());
                    angleCos = MathHelper.Clamp(angleCos, -1, 1);

                    float playerAngle = (float)Math.Acos(angleCos);
                    if (playerAngle <= halfConeAngle)
                    {
                        visibleBoids.Add(boids[i]);
                    }
                }
            }

            return visibleBoids;
        }
        public void FixPosition()
        {
            if (X > Game.Window.Width + HalfWidth) X = -HalfWidth;
            else if (X < -HalfWidth) X = Game.Window.Width + HalfWidth;


            if (Y > Game.Window.Height + HalfHeight) Y = -HalfHeight;
            else if (Y < -HalfHeight) Y = Game.Window.Height + HalfHeight;
        }
        public void Draw()
        {
            sprite.DrawTexture(texture);
        }
        static public void ChangeWeights()
        {
            float value = 0.000005f;

            if (Game.Window.GetKey(KeyCode.A))
            {
                if (Game.Window.GetKey(KeyCode.Up))
                {
                    alignmentWeight += value;
                }
                else if (Game.Window.GetKey(KeyCode.Down))
                {
                    alignmentWeight -= value;
                }
                alignmentWeight = MathHelper.Clamp(alignmentWeight, 0, Delta * 10);
                Game.texts[0].SetText("Alignment: " + (int)(alignmentWeight / Delta));

                return;
            }

            else if (Game.Window.GetKey(KeyCode.C))
            {
                if (Game.Window.GetKey(KeyCode.Up))
                {
                    cohesionWeight += value;
                }
                else if (Game.Window.GetKey(KeyCode.Down))
                {
                    cohesionWeight -= value;

                }
                cohesionWeight = MathHelper.Clamp(cohesionWeight, 0, Delta * 10);
                Game.texts[1].SetText("Cohesion: " + (int)(cohesionWeight / Delta));
                return;
            }

            else if (Game.Window.GetKey(KeyCode.S))
            {
                if (Game.Window.GetKey(KeyCode.Up))
                {
                    separationWeight += value;
                }
                else if (Game.Window.GetKey(KeyCode.Down))
                {
                    separationWeight -= value;
                }
                separationWeight = MathHelper.Clamp(separationWeight, 0, Delta * 10);
                Game.texts[2].SetText("Separation: " + (int)(separationWeight / Delta));
                return;
            }
            else if (Game.Window.GetKey(KeyCode.V))
            {
                if (Game.Window.GetKey(KeyCode.Up))
                {
                    halfConeAngle = MathHelper.RadiansToDegrees(halfConeAngle);
                    halfConeAngle+= 0.05f;
                    halfConeAngle = MathHelper.DegreesToRadians(halfConeAngle);
                }
                else if (Game.Window.GetKey(KeyCode.Down))
                {
                    halfConeAngle = MathHelper.RadiansToDegrees(halfConeAngle);
                    halfConeAngle -= 0.05f;
                    halfConeAngle = MathHelper.DegreesToRadians(halfConeAngle);
                }
                halfConeAngle = MathHelper.Clamp(halfConeAngle, 0, (float)Math.PI);
                Game.texts[3].SetText("Vision Angle: " + (int)(HalfConeAngle)*2);
                return;
            }
        }

        /* METODI ROTTI
         * 
            //Velocity = Vector2.Lerp(Velocity, (Velocity + alignment * alignmentWeight + cohesion * cohesionWeight + separation * separationWeight*2).Normalized(),Game.DeltaTime * 2);
         * 
         * 
        public void Cohesion()
        {
            List<Boid> boids = VisibleCohesionBoids();

            if (boids.Count < 1) return;

            Vector2 averagePosition = GetCohesion(boids);

            float alpha = sprite.Rotation;

            float beta = (float)Math.Atan2(averagePosition.Y, averagePosition.X);

            float difference = beta - alpha;

            sprite.Rotation += difference * Game.DeltaTime * 2;

        }

        public void Separation()
        {
            List<Boid> boids = VisibleSeparationBoids();

            counterSeparation -= Game.DeltaTime;

            if (boids.Count < 1) return;

            if (counterSeparation > 0) return;

            Vector2 averagePosition = GetSeparation(boids);//get's the opposite vector of the average vector

            float alpha = sprite.Rotation;

            //fix that shit below

            float beta = (float)Math.Atan2(averagePosition.Y, averagePosition.X);

            float difference = beta - alpha;

            //Forward = averagePosition;
            sprite.Rotation += difference * Game.DeltaTime * 500;

            //flySpeed = 0;

            counterSeparation = timeToSeparation;
        }

        public void Alignment()
        {
           // List<Boid> boids = VisibleAlignmentBoids();
           List<Boid> boids = Game.Boids;

            if (boids.Count < 1) return;

            Vector2 averageForward = GetAlignment(boids);

            float alpha = sprite.Rotation;

            float beta = (float)Math.Atan2(averageForward.Y, averageForward.X);

            float difference = beta - alpha;

            //sprite.Rotation += difference * Game.DeltaTime;
            //sprite.Rotation = difference;

            Forward = averageForward;

            /* double dot = Vector2.Dot(Forward, averageForward); // dot of the average forward of other boids

             dot = MathHelper.Clamp(dot, -1, 1); //cosene of the dot

             float angle = (float)Math.Acos(dot);
             float oneDegree = MathHelper.DegreesToRadians(1);
             float sign = Math.Sign(dot);

             float X = Forward.X + (float)Math.Cos(angle) * Game.DeltaTime * sign;
             float Y = Forward.Y + (float)Math.Sin(angle) * Game.DeltaTime * sign;

             Forward = new Vector2(X,Y);
        }*/
        /*public List<Player> GetVisiblePlayers()
        {
            List<Player> players = ((PlayScene)Game.CurrentScene).Players;
            List<Player> visiblePlayers = new List<Player>();

            for (int i = 0; i < players.Count; i++)
            {
                if (!players[i].IsAlive) continue;

                Vector2 dist = players[i].Position - this.Position;
                if (dist.LengthSquared < VisionRay * VisionRay)
                {
                    float angleCos = Vector2.Dot(Forward, dist.Normalized());
                    angleCos = MathHelper.Clamp(angleCos, -1, 1);

                    float playerAngle = (float)Math.Acos(angleCos);
                    if (playerAngle <= halfConeAngle)
                    {
                        visiblePlayers.Add(players[i]);
                    }
                }
            }

            return visiblePlayers;
        }*/
        /*
         * 
        public Vector2 GetSeparation1(List<Boid> boids)
        {
            if (boids.Count < 1) return Vector2.Zero;

            //we have at least 1 boids
            Vector2 averagePosition = Vector2.Zero;

            for (int i = 0; i < boids.Count; i++)
            {
                averagePosition += boids[i].Position;
            }

            averagePosition /= boids.Count;

            float angle = -(float)Math.Atan2(averagePosition.Y, averagePosition.X);

            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }
         * 
         * 
         * 
         * public List<Boid> VisibleAlignmentBoids()
{
    List<Boid> boids = Game.Boids;
    List<Boid> visibleBoids = new List<Boid>();

    for (int i = 0; i < boids.Count; i++)
    {
        if (boids[i] == this) continue;

        Vector2 dist = boids[i].Position - Position;

        float alignmentRaySquared = AlignmentRay * AlignmentRay;

        if (dist.LengthSquared < alignmentRaySquared)
        {
            visibleBoids.Add(boids[i]);
        }
    }

    return visibleBoids;
}
public List<Boid> VisibleCohesionBoids()
{
    List<Boid> boids = Game.Boids;
    List<Boid> visibleBoids = new List<Boid>();

    for (int i = 0; i < boids.Count; i++)
    {
        if (boids[i] == this) continue;

        Vector2 dist = boids[i].Position - Position;

        float cohesionRayRaySquared = CohesionRay * CohesionRay;

        if (dist.LengthSquared < cohesionRayRaySquared)
        {
            visibleBoids.Add(boids[i]);
        }
    }

    return visibleBoids;
}
public List<Boid> VisibleSeparationBoids()
{
    List<Boid> boids = Game.Boids;
    List<Boid> visibleBoids = new List<Boid>();

    for (int i = 0; i < boids.Count; i++)
    {
        if (boids[i] == this) continue;

        Vector2 dist = boids[i].Position - Position;

        float separationRaySquared = SeparationRay * SeparationRay;

        if (dist.LengthSquared < separationRaySquared)
        {
            visibleBoids.Add(boids[i]);
        }
    }

    return visibleBoids;
}*/

    }
}
