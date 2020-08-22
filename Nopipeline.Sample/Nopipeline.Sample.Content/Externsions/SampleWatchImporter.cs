using Microsoft.Xna.Framework.Content.Pipeline;
using System.IO;

namespace Nopipeline.Sample.Content.Extensions
{
	[ContentImporter(".sampleWatch", DisplayName = "Sample Watch Importer", DefaultProcessor = "PassThroughProcessor")]
	public class SampleWatchImporter : ContentImporter<string>
	{
		public override string Import(string filename, ContentImporterContext context)
		{
			// 
			return File.ReadAllText(Path.ChangeExtension(filename, ".txt"));
		}
	}
}
