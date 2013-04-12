# Populr.me API .NET

We've created a wrapper for the Populr API that makes it easy to use in .NET environments. To get started, take a look at the files in the Populr folder. You can add this project to your workspace as a dependency, or integrate the files directly into your project. The source makes extensive use of RestSharp, an open-source solution for REST service interfaces in .NET.

The PopulrAPIConsole project shows an example use of the API. The important parts of the sample are explained below.

## Performing Common Tasks

### Initialization

      PopulrAPI populr = new PopulrAPI ("LIIZAOPK-ITFEAWEB-HJYCKMPQ");

### Creating an Asset

			FileStream stream = new FileStream("/my_picture.jpg", FileMode.Open);
			Asset imageAsset = populr.createImageAsset(stream, "My Profile Picture", "http://www.apple.com/");
			stream.Close ();

### Listing Available Templates

			List<PopTemplate> templates = populr.getTemplates();
			Console.WriteLine ("Found " + templates.Count + " templates");

### Creating a Pop based on a Template

      // Note: All pops must be created based on an existing template setup in your Populr account.
      Pop newPop = new Pop(templates[0]);

### Populating Pops

      // Basic attributes can be modified directly.
      newPop.title = "Wow a new pop!";

      // Always check to make sure regions and tags exist before populating them.
			// Trying to populate a region that does not exist will result in an
      // APIException being thrown.
			if (newPop.HasUnpopulatedTag("personal_site"))
				newPop.PopulateTag("personal_site", "http://www.gotow.net/");

			if (newPop.HasUnpopulatedRegion("profile_image_region")) {
        newPop.PopulateRegion("profile_image_region", imageAsset);
  		}

      // Synchronously saves changes. An APIException will be thrown if validation fails.
      // For example, if you've assigned the pop the same slug as another pop.
      newPop.Save ();

### Publishing a Pop

      // Publishes the pop immediately. After calling this method, the pop's published_pop_url
      // is set populated.
      newPop.Publish();


## We welcome pull requests!
