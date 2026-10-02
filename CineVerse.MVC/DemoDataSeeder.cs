using CineVerse.Application.Common;
using CineVerse.Domain.Entities;
using CineVerse.Infrastructure.Persistence.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CineVerse.MVC;

/// <summary>
/// Geliştirme ortamında siteyi dolu göstermek için örnek veriler: kullanıcılar, türler,
/// etiketler, filmler (afişleriyle) ve yorumlar.
///
/// Sadece bir kez çalışır: ilk demo kullanıcı veritabanında varsa hiçbir şey yapmaz.
/// Senin elle eklediğin tür/etiket/filmleri tekrar oluşturmaz, isim eşleşirse onları kullanır.
/// Afişler: wwwroot/images/posters (CineVerse için üretilmiş özgün demo afişleri).
/// </summary>
public static class DemoDataSeeder
{
    private const string DemoPassword = "Demo@123";

    private sealed record DemoUser(string UserName, string FirstName, string LastName, string Email);

    private sealed record DemoMovie(
        string Title, int Year, string Director, string Genre, string? Poster,
        bool InTheaters, int AddedDaysAgo, string[] Tags, string Overview);

    private sealed record DemoReview(string Movie, string User, int Rating, int DaysAgo, string Body);

    public static async Task SeedAsync(IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

        try
        {
            // Demo veriler daha önce eklendiyse hiçbir şey yapma.
            // (Böylece admin panelinden sildiğin bir demo filmi/yorumu bir sonraki açılışta geri gelmez.)
            if (await userManager.FindByNameAsync(Users[0].UserName) is not null)
                return;

            var users = await SeedUsersAsync(userManager);
            var genres = await SeedGenresAsync(context);
            var tags = await SeedTagsAsync(context);
            var movies = await SeedMoviesAsync(context, genres, tags);
            await SeedReviewsAsync(context, movies, users);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Demo data error: " + ex.Message);
        }
    }

    // ---------------------------------------------------------------- Kullanıcılar

    private static async Task<Dictionary<string, Guid>> SeedUsersAsync(UserManager<AppUser> userManager)
    {
        var result = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);

        foreach (var demo in Users)
        {
            var user = await userManager.FindByNameAsync(demo.UserName);
            if (user is null)
            {
                user = new AppUser
                {
                    UserName = demo.UserName,
                    FirstName = demo.FirstName,
                    LastName = demo.LastName,
                    Email = demo.Email,
                    EmailConfirmed = true,
                    DoB = new DateOnly(1995, 1, 1)
                };

                var created = await userManager.CreateAsync(user, DemoPassword);
                if (!created.Succeeded)
                {
                    Console.WriteLine($"Demo user '{demo.UserName}': " + string.Join(",", created.Errors.Select(e => e.Description)));
                    continue;
                }

                await userManager.AddToRoleAsync(user, Roles.User);
            }

            result[demo.UserName] = user.Id;
        }

        return result;
    }

    // ---------------------------------------------------------------- Türler ve etiketler

    private static async Task<Dictionary<string, Guid>> SeedGenresAsync(AppDbContext context)
    {
        var existing = await context.Genres.ToListAsync();
        var result = existing
            .GroupBy(g => g.Name, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First().Id, StringComparer.OrdinalIgnoreCase);

        foreach (var name in Movies.Select(m => m.Genre).Distinct())
        {
            if (result.ContainsKey(name))
                continue;

            var genre = new Genre { Name = name, Slug = name.ToKebabCase() };
            context.Genres.Add(genre);
            result[name] = genre.Id;
        }

        await context.SaveChangesAsync();
        return result;
    }

    private static async Task<Dictionary<string, Guid>> SeedTagsAsync(AppDbContext context)
    {
        var existing = await context.Tags.ToListAsync();
        var result = existing
            .GroupBy(t => t.Name, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(t => t.Key, t => t.First().Id, StringComparer.OrdinalIgnoreCase);

        foreach (var name in Movies.SelectMany(m => m.Tags).Distinct())
        {
            if (result.ContainsKey(name))
                continue;

            var tag = new Tag { Name = name, Slug = name.ToKebabCase() };
            context.Tags.Add(tag);
            result[name] = tag.Id;
        }

        await context.SaveChangesAsync();
        return result;
    }

    // ---------------------------------------------------------------- Filmler

    private static async Task<Dictionary<string, Guid>> SeedMoviesAsync(
        AppDbContext context, Dictionary<string, Guid> genres, Dictionary<string, Guid> tags)
    {
        var existing = await context.Movies.Select(m => new { m.Id, m.Title }).ToListAsync();
        var result = existing
            .GroupBy(m => m.Title, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(m => m.Key, m => m.First().Id, StringComparer.OrdinalIgnoreCase);

        var now = DateTimeOffset.UtcNow;

        foreach (var demo in Movies)
        {
            if (result.ContainsKey(demo.Title))
                continue;

            var addedOn = now.AddDays(-demo.AddedDaysAgo);
            var movie = new Movie
            {
                GenreId = genres[demo.Genre],
                Title = demo.Title,
                Slug = demo.Title.ToKebabCase(),
                Director = demo.Director,
                ReleaseYear = demo.Year,
                PosterUrl = demo.Poster is null ? null : $"/images/posters/{demo.Poster}.jpg",
                Overview = demo.Overview,
                IsInTheaters = demo.InTheaters,
                CreatedOn = addedOn,
                LastModifiedOn = addedOn
            };

            foreach (var tagName in demo.Tags)
                movie.Tags.Add(new MovieTag { TagId = tags[tagName], CreatedOn = addedOn, LastModifiedOn = addedOn });

            context.Movies.Add(movie);
            result[demo.Title] = movie.Id;
        }

        await context.SaveChangesAsync();
        return result;
    }

    // ---------------------------------------------------------------- Yorumlar

    private static async Task SeedReviewsAsync(
        AppDbContext context, Dictionary<string, Guid> movies, Dictionary<string, Guid> users)
    {
        // Aynı kullanıcı aynı filme ikinci kez yorum yazmasın
        var existingPairs = (await context.Reviews
                .Select(r => new { r.MovieId, r.UserId })
                .ToListAsync())
            .Select(r => (r.MovieId, r.UserId))
            .ToHashSet();

        var now = DateTimeOffset.UtcNow;
        var index = 0;

        foreach (var demo in Reviews)
        {
            index++;
            if (!movies.TryGetValue(demo.Movie, out var movieId) || !users.TryGetValue(demo.User, out var userId))
                continue;

            if (!existingPairs.Add((movieId, userId)))
                continue;

            var writtenOn = now.AddDays(-demo.DaysAgo).AddHours(-(index % 7) * 3);
            context.Reviews.Add(new Review
            {
                MovieId = movieId,
                UserId = userId,
                Rating = demo.Rating,
                Body = demo.Body,
                CreatedOn = writtenOn,
                LastModifiedOn = writtenOn
            });
        }

        await context.SaveChangesAsync();
    }

    // ================================================================ Veriler

    private static readonly DemoUser[] Users =
    [
        new("cinephile_ada", "Ada", "Yilmaz", "ada@example.com"),
        new("popcorn_pete", "Peter", "Hall", "pete@example.com"),
        new("reel_maria", "Maria", "Lopez", "maria@example.com"),
        new("nightowl_kaan", "Kaan", "Demir", "kaan@example.com"),
        new("lena_watches", "Lena", "Fischer", "lena@example.com"),
        new("deniz_frames", "Deniz", "Arslan", "deniz@example.com"),
    ];

    private static readonly DemoMovie[] Movies =
    [
        new("The Godfather", 1972, "Francis Ford Coppola", "Crime", "the-godfather", false, 40,
            ["Oscar Winner", "Classic"],
            "The aging patriarch of the Corleone crime family hands control of his empire to his reluctant youngest son, Michael, who is slowly transformed by the world he once wanted no part of."),
        new("Pulp Fiction", 1994, "Quentin Tarantino", "Crime", "pulp-fiction", false, 38,
            ["Oscar Winner", "Classic"],
            "Two hitmen, a boxer, a gangster's wife and a pair of diner robbers cross paths across Los Angeles in a series of darkly funny, interlocking stories told out of order."),
        new("The Shining", 1980, "Stanley Kubrick", "Horror", null, false, 36,
            ["Classic"],
            "Jack Torrance takes a job as the winter caretaker of the isolated Overlook Hotel, bringing his wife and young son along. As the snow closes in, the hotel's dark influence takes hold of him."),
        new("The Lion King", 1994, "Roger Allers, Rob Minkoff", "Animation", "the-lion-king", false, 35,
            ["Oscar Winner", "Animal Adventure", "Family", "Classic"],
            "Young lion prince Simba flees his kingdom after his father's death, believing himself responsible, and must one day return to face his past and claim his place in the circle of life."),
        new("Spirited Away", 2001, "Hayao Miyazaki", "Animation", "spirited-away", false, 33,
            ["Oscar Winner", "Family"],
            "On the way to her new home, ten-year-old Chihiro wanders into a world of spirits. When her parents are transformed, she must work in a magical bathhouse to find a way to save them."),
        new("The Dark Knight", 2008, "Christopher Nolan", "Action", "the-dark-knight", false, 30,
            ["Oscar Winner"],
            "Batman, Lieutenant Gordon and District Attorney Harvey Dent set out to dismantle Gotham's mob, until a chaotic criminal known as the Joker pushes the whole city to its breaking point."),
        new("Inception", 2010, "Christopher Nolan", "Science Fiction", "inception", false, 28,
            ["Oscar Winner", "Mind-Bending"],
            "Dom Cobb steals secrets from deep inside people's dreams. Offered a chance to erase his criminal past, he must pull off the reverse: plant an idea in a target's mind without him ever noticing."),
        new("Interstellar", 2014, "Christopher Nolan", "Science Fiction", "interstellar", false, 26,
            ["Oscar Winner", "Space"],
            "With Earth's crops failing, a former NASA pilot leaves his children behind to lead a mission through a wormhole, searching for a new home for humanity among distant stars."),
        new("Whiplash", 2014, "Damien Chazelle", "Drama", "whiplash", false, 24,
            ["Oscar Winner", "Music"],
            "An ambitious young jazz drummer enrolls in an elite music conservatory, where a ruthless instructor pushes him far beyond his limits in pursuit of greatness."),
        new("The Grand Budapest Hotel", 2014, "Wes Anderson", "Comedy", "the-grand-budapest-hotel", false, 22,
            ["Oscar Winner", "Quirky Comedy"],
            "The legendary concierge of a famous European hotel and his loyal lobby boy are swept into a wild adventure involving a priceless painting, a stolen fortune and a murder."),
        new("Gone Girl", 2014, "David Fincher", "Psychological Thriller", null, false, 21,
            ["Mind-Bending"],
            "On the morning of their fifth wedding anniversary, Nick Dunne's wife Amy disappears. As the media circus grows and secrets come to light, everyone starts to wonder whether he is responsible."),
        new("Mad Max: Fury Road", 2015, "George Miller", "Action", "mad-max-fury-road", false, 20,
            ["Oscar Winner"],
            "In a post-apocalyptic wasteland, the drifter Max teams up with the rebel warrior Furiosa to help a group of women escape a tyrant in a relentless chase across the desert."),
        new("La La Land", 2016, "Damien Chazelle", "Love", "la-la-land", false, 18,
            ["Oscar Winner", "Music"],
            "An aspiring actress and a dedicated jazz pianist fall in love in Los Angeles, but as their careers begin to take off, their dreams start to pull them in different directions."),
        new("Get Out", 2017, "Jordan Peele", "Horror", "get-out", false, 16,
            ["Oscar Winner", "Mind-Bending"],
            "A young Black man visits his white girlfriend's family estate for the weekend. Their overly warm welcome soon gives way to a series of disturbing discoveries."),
        new("Paddington 2", 2017, "Paul King", "Comedy", "paddington-2", false, 14,
            ["Family", "Animal Adventure", "Slapstick"],
            "Paddington finds the perfect birthday present for Aunt Lucy in an antique shop, but when the book is stolen and he is wrongly blamed, the Brown family sets out to clear his name."),
        new("Parasite", 2019, "Bong Joon Ho", "Psychological Thriller", "parasite", false, 12,
            ["Oscar Winner"],
            "The penniless Kim family cleverly works its way into the lives of the wealthy Park household, one job at a time, until an unexpected discovery turns their scheme into something far darker."),
        new("Knives Out", 2019, "Rian Johnson", "Mystery", "knives-out", false, 10,
            ["Whodunit"],
            "When a wealthy crime novelist is found dead after his 85th birthday party, the eccentric detective Benoit Blanc arrives to investigate a family where everyone has a motive."),
        new("Oppenheimer", 2023, "Christopher Nolan", "Drama", "oppenheimer", true, 5,
            ["Oscar Winner", "Based on True Events"],
            "The story of J. Robert Oppenheimer, the physicist who led the Manhattan Project, and the personal and political cost of building the weapon that changed the world forever."),
        new("Inside Out 2", 2024, "Kelsey Mann", "Animation", "inside-out-2", true, 3,
            ["Family"],
            "Riley is now a teenager, and the emotions running Headquarters are about to get company. When Anxiety and a few new feelings move in, Joy and the others fight to protect who Riley is."),
        new("Dune: Part Two", 2024, "Denis Villeneuve", "Science Fiction", "dune-part-two", true, 2,
            ["Oscar Winner", "Space"],
            "Paul Atreides joins Chani and the Fremen on the desert planet Arrakis, seeking revenge on the conspirators who destroyed his family while trying to escape a terrible future only he can foresee."),
    ];

    private static readonly DemoReview[] Reviews =
    [
        // Inception
        new("Inception", "cinephile_ada", 5, 27, "Every rewatch reveals something new. The hallway fight still blows my mind."),
        new("Inception", "popcorn_pete", 4, 25, "Clever and loud in the best way. Took me a while to keep track of the dream levels."),
        new("Inception", "reel_maria", 5, 22, "That ending sparked a two hour debate at dinner. Worth it."),
        new("Inception", "nightowl_kaan", 5, 18, "Watched it at 2am and could not sleep afterwards. The score is unreal."),
        new("Inception", "lena_watches", 4, 9, "Brilliant concept, a little heavy on exposition in the first hour."),

        // Dune: Part Two
        new("Dune: Part Two", "cinephile_ada", 5, 1, "Pure spectacle. Saw it on the biggest screen I could find and do not regret it."),
        new("Dune: Part Two", "deniz_frames", 5, 1, "The sound design alone deserves a standing ovation."),
        new("Dune: Part Two", "nightowl_kaan", 4, 1, "Stunning visuals. Pacing drags a bit in the middle but the finale makes up for it."),
        new("Dune: Part Two", "lena_watches", 5, 0, "Better than the first one, and I loved the first one."),
        new("Dune: Part Two", "popcorn_pete", 4, 0, "Big, bold and beautiful. Bring snacks, it is long."),

        // Interstellar
        new("Interstellar", "reel_maria", 5, 24, "I cried at the video messages scene. Every single time."),
        new("Interstellar", "deniz_frames", 5, 20, "Science, family and an organ soundtrack that shakes the room."),
        new("Interstellar", "popcorn_pete", 4, 15, "Some of the physics went over my head but the emotions landed perfectly."),
        new("Interstellar", "nightowl_kaan", 5, 7, "My favourite space movie, no contest."),

        // Oppenheimer
        new("Oppenheimer", "lena_watches", 5, 4, "Three hours that felt like ninety minutes. Gripping from start to finish."),
        new("Oppenheimer", "cinephile_ada", 4, 3, "Dense and dialogue heavy, but the performances carry it."),
        new("Oppenheimer", "reel_maria", 5, 2, "The silence in that test scene is something I will never forget."),
        new("Oppenheimer", "deniz_frames", 4, 1, "Not an easy watch, but an important one."),

        // Parasite
        new("Parasite", "deniz_frames", 5, 11, "Funny, tense and heartbreaking, sometimes all in the same scene."),
        new("Parasite", "cinephile_ada", 5, 10, "The less you know going in, the better. Just watch it."),
        new("Parasite", "nightowl_kaan", 5, 8, "That staircase symbolism lives rent free in my head."),
        new("Parasite", "popcorn_pete", 4, 5, "Did not expect the second half at all. What a turn."),

        // Spirited Away
        new("Spirited Away", "lena_watches", 5, 30, "Pure magic. Every frame looks like a painting."),
        new("Spirited Away", "reel_maria", 5, 26, "I watched it as a kid and again as an adult. Still perfect."),
        new("Spirited Away", "popcorn_pete", 4, 19, "Wonderfully strange. No Face is my spirit animal."),
        new("Spirited Away", "deniz_frames", 5, 12, "The train scene is one of the calmest, most beautiful moments in cinema."),

        // The Dark Knight
        new("The Dark Knight", "nightowl_kaan", 5, 28, "The best superhero movie ever made, and it is barely a superhero movie."),
        new("The Dark Knight", "popcorn_pete", 5, 21, "Heath Ledger is terrifying. The interrogation scene is perfect."),
        new("The Dark Knight", "lena_watches", 4, 13, "Great crime story. The last act is a little crowded."),

        // Whiplash
        new("Whiplash", "deniz_frames", 5, 23, "The final ten minutes made me forget to breathe."),
        new("Whiplash", "reel_maria", 4, 17, "Intense and uncomfortable in the best way. Not quite my tempo though."),
        new("Whiplash", "nightowl_kaan", 5, 6, "I have never been this stressed by a drum solo."),

        // La La Land
        new("La La Land", "reel_maria", 5, 17, "Bittersweet and gorgeous. That epilogue destroyed me."),
        new("La La Land", "popcorn_pete", 3, 14, "Beautiful to look at, but musicals are not really my thing."),
        new("La La Land", "lena_watches", 4, 8, "The colours, the music, the planetarium scene. Lovely."),

        // The Godfather
        new("The Godfather", "cinephile_ada", 5, 39, "A masterpiece. Every scene is a lesson in filmmaking."),
        new("The Godfather", "nightowl_kaan", 5, 33, "Slow at first, then you realise you cannot look away."),
        new("The Godfather", "deniz_frames", 4, 25, "Classic for a reason. The wedding opening is iconic."),

        // Inside Out 2
        new("Inside Out 2", "lena_watches", 5, 2, "Anxiety is the most relatable character Pixar has ever made."),
        new("Inside Out 2", "reel_maria", 4, 1, "Funny and surprisingly emotional. Took my little sister and we both loved it."),
        new("Inside Out 2", "popcorn_pete", 4, 0, "A worthy sequel. The hockey camp setting works really well."),

        // The Grand Budapest Hotel
        new("The Grand Budapest Hotel", "cinephile_ada", 5, 20, "Every frame is perfectly symmetrical and somehow it never feels cold."),
        new("The Grand Budapest Hotel", "deniz_frames", 4, 15, "Charming, fast and hilarious. The pastel colours are a treat."),

        // Paddington 2
        new("Paddington 2", "popcorn_pete", 5, 12, "Impossible to watch without smiling. A perfect family film."),
        new("Paddington 2", "lena_watches", 5, 9, "Kind, clever and very funny. The prison kitchen scenes are gold."),

        // Knives Out
        new("Knives Out", "nightowl_kaan", 4, 9, "A really fun whodunit with a great ensemble cast."),
        new("Knives Out", "reel_maria", 5, 6, "Twists I did not see coming. That sweater deserves its own award."),

        // Get Out
        new("Get Out", "deniz_frames", 5, 15, "Smart, creepy and full of details you only notice the second time."),
        new("Get Out", "cinephile_ada", 4, 11, "The tea cup scene gave me chills."),

        // Pulp Fiction
        new("Pulp Fiction", "nightowl_kaan", 5, 37, "Iconic dialogue from start to finish. The dance scene is legendary."),
        new("Pulp Fiction", "popcorn_pete", 4, 29, "Messy timeline, perfect characters. Very quotable."),

        // Mad Max: Fury Road
        new("Mad Max: Fury Road", "lena_watches", 5, 19, "Two hours of pure adrenaline. The practical effects are incredible."),
        new("Mad Max: Fury Road", "popcorn_pete", 5, 16, "One long chase scene and somehow it never gets boring."),

        // The Lion King
        new("The Lion King", "reel_maria", 5, 34, "My childhood in one movie. The opening song still gives me goosebumps."),
        new("The Lion King", "deniz_frames", 4, 27, "Timeless story, great soundtrack."),

        // The Shining
        new("The Shining", "nightowl_kaan", 4, 35, "Slow burn horror at its finest. Do not watch it alone at night."),

        // Gone Girl
        new("Gone Girl", "cinephile_ada", 4, 20, "Twisted and gripping. Never trust a narrator."),

        // Senin eklediğin filmler (varsa onlara da birkaç yorum)
        new("Shutter Island", "nightowl_kaan", 5, 2, "That final line changes everything. Had to rewatch it the next day."),
        new("Shutter Island", "lena_watches", 4, 1, "Moody and atmospheric. I guessed the twist but still enjoyed the ride."),
        new("Five Feet Apart", "reel_maria", 4, 1, "Sweet and sad. Bring tissues."),
    ];
}
