using ChaiFoxes.FMODAudio;


namespace Resources
{
	public static class Sounds
	{
		
		public static Sound MainBaseLayer;
		public static Sound MainTopLayer;

		public static Sound ButtonPress;
		public static Sound ButtonRelease;
		public static Sound Cannon;
		public static Sound Slide;
		public static Sound Crouch;
		public static Sound Jump;
		public static Sound Checkpoint;
		public static Sound Death;

		public static Sound CatPickup1;
		public static Sound CatPickup2;
		public static Sound CatPickup3;

		public static Sound FrogPickup;


		public static void Load()
		{
			MainBaseLayer = AudioMgr.LoadStreamedSound("Music/MainBaseLayer.ogg");
			MainBaseLayer.Looping = true;
			MainTopLayer = AudioMgr.LoadStreamedSound("Music/MainTopLayer.ogg");
			MainTopLayer.Looping = true;
			
			ButtonPress = Load3DSound("Sounds/ButtonPress.wav", 700, 900);
			ButtonRelease = Load3DSound("Sounds/ButtonRelease.wav", 400, 900);
			Cannon = Load3DSound("Sounds/Cannon.wav", 700, 900);
			Cannon.Volume = 0.2f;


			Slide = AudioMgr.LoadSound("Sounds/Slide.wav");
			Slide.Looping = true;
			Slide.LowPass = 0.5f;

			Crouch = AudioMgr.LoadSound("Sounds/Crouch.wav");
			Crouch.LowPass = 0.5f;

			Jump = AudioMgr.LoadSound("Sounds/Jump.wav");
			
			Checkpoint = AudioMgr.LoadSound("Sounds/Checkpoint.wav");
			
			Death = Load3DSound("Sounds/Death.wav", 700, 900);
			CatPickup1 = AudioMgr.LoadSound("Sounds/CatPickup1.wav");
			CatPickup2 = AudioMgr.LoadSound("Sounds/CatPickup2.wav");
			CatPickup3 = AudioMgr.LoadSound("Sounds/CatPickup3.wav");
			
			FrogPickup = AudioMgr.LoadSound("Sounds/FrogPickup.wav");
			
		}

		static Sound Load3DSound(string path, float minDistance, float maxDistance)
		{
			var sound = AudioMgr.LoadSound(path);
			sound.Is3D = true;
			sound.MinDistance3D = minDistance;
			sound.MaxDistance3D = maxDistance;

			return sound;
		}

		public static void Unload()
		{
		}

	}
}
