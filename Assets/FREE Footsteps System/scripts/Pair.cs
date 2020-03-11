// - AUTHOR : Pavel Cristian.
// - WHERE SHOULD BE ATTACHED : This script should be attached on the main root of the character, 
//	 on the GameObject the Rigidbody / CharacterController script is attached.
// - PURPOSE OF THE SCRIPT : The purpose of this script is to gather data from the ground below the character and use the
//   data to find a user-defined sound for the type of ground found.

// DISCLAIMER : THIS SCRIPT CAN BE USED IN ANY WAY, MENTIONING MY WORK WILL BE GREATLY APPRECIATED BUT NOT REQUIRED.

namespace Footsteps
{

    public partial class CharacterFootsteps
    {
        public class Pair<T1,T2>
        {
            public T1 Left { get; set; }
            public T2 Right { get; set; }
        }
    }
}
