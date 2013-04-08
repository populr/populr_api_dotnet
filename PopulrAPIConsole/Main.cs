using System;
using Populr;
using System.IO;
using System.Collections.Generic;

namespace PopulrAPIConsole
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			PopulrAPI populr = new PopulrAPI ("LIIZAOPK-ITFEAWEB-HJYCKMPQ");

			// Create a new pop from a template
			List<PopTemplate> templates = populr.getTemplates();
			Console.WriteLine ("Found " + templates.Count + " templates");
			Pop newPop = new Pop(templates[0]);
			newPop.title = "Wow a new pop!";

			// Populate regions that are empty
			if (p.HasUnpopulatedTag("personal_site"))
				p.PopulateTag("personal_site", "http://www.gotow.net/");
			
			if (p.HasUnpopulatedRegion("profile_image_region")) {
				FileStream stream = new FileStream("/469118_10150714650073758_419024550_o.jpg", FileMode.Open);
				Asset imageAsset = populr.createImageAsset(stream, "My Profile Picture", "http://www.apple.com/");
				p.PopulateRegion("profile_image_region", imageAsset);
				stream.Close ();
			}

			newPop.Save ();
			Console.WriteLine("Created new pop from template 0: " + newPop._id);

			newPop.Publish();
		}
	}
}