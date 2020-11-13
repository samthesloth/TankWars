using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TankWars;

namespace Model
{

    [JsonObject(MemberSerialization.OptIn)]
    class Wall
    {
        [JsonProperty(PropertyName = "wall")]
        private int ID;

        [JsonProperty(PropertyName = "p1")]
        private Vector2D topLeft;

        [JsonProperty(PropertyName = "p2")]
        private Vector2D bottomRight;

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
