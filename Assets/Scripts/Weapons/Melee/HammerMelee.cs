using Scripts.Weapons;

namespace Scripts
{
    public class HammerMelee : Melee
    {
        protected override void OnImpact(string affected)
        {
            switch (affected)
            {
                case "Player":
                    Power -= 5;
                    break;
                case "Floor":
                    Power -= 10;
                    if(IsServer)
                        SpawnStanSphere();
                    break;
            }
        }

        protected void SpawnStanSphere()
        {
            
        }
    }
}
