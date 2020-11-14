using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TankWars;

namespace Model
{

    [JsonObject(MemberSerialization.OptIn)]
    public class Wall
    {
        [JsonProperty(PropertyName = "wall")]
        public int ID
        {
            get;
        }

        [JsonProperty(PropertyName = "p1")]
        public Vector2D topLeft
        {
            get;
        }

        [JsonProperty(PropertyName = "p2")]
        public Vector2D bottomRight
        {
            get;
        }

        public Wall()
        {

        }

        public Wall(int ID, Vector2D topLeft, Vector2D bottomRight)
        {
            this.ID = ID;
            this.topLeft = topLeft;
            this.bottomRight = bottomRight;
        }
    }
}
