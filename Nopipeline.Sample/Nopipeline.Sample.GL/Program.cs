using System;

namespace Nopipeline.Sample.GL
{
	public static class Program
	{
		[STAThread]
		static void Main()
		{
			using (var game = new Game1())
			{
				game.Run();
			}
		}
	}
}
