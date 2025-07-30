# Unseen TopScore string submission web app

## Approach Taken:

- Scaffolded single ASP.NET Razor Pages .NET 9 app from official template
- Implemented `DataService` to hold core functionality
- Utilises `ReadOnlySpan<char>` to reduce allocation overhead from string processing

## Tools Used:

- Visual Studio Code
- GitHub Copilot

## Ai Disclaimer:

Core logic was written solely by me (the word extraction), though the lines are blurred somewhat these days with Copilot suggestion and next-edit flows.

Agent Mode was engaged to assist with building out the basics of the search as that's mostly boilerplate in these sorts of things, and with helping to slightly pretty things up somewhat (CSS, bootstrap etc).

Assistance wasn't restricted, so apologies if I was wrong on that.

## Running the solution:

You should be able to just `dotnet run` the project, it should take care of creating the SQLite database for you.

## Assumptions made:

### Simple structure

The brief is pretty sparse, so I didn't see the point in spinning out a separate API & frontend.

I also chose to go with Razor Pages as it seemed the simplest thing to get up and running with.

### Random candidate selection

Though it nagged at me (I wanted to do much more, but restrained myself), I chose to simply have it select a random candidate if more than one was found rather than test each and select one not present in the database already.

An option I explored was the possibility of presenting candidates to the user to pick from (once excluded), future enhancement opportunity I guess.

### ReadOnlySpan usage

It's likely overkill for the toy implementation, but wanted to demonstrate something beyond a simple Linq split->filter->group->take approach.

I held myself back from throwing some parallelism in there too.

Mostly went with this approach because strings are expensive allocation wise, and with arbitrarily long strings and a possibility of multiple users, you could end up with a lot of allocations (especially if this was behind an API) - again, likely overkill, but a fun challenge.

## Improvements

Here's some of the things I'd possibly have done differently:

- Live validation & candidate display
  - It would be cool to have the submission page show you the candidates / why certain words were not picked
- Candidate selection based on stored words
  - Rather than the random selection approach I took, it could have been better by considering which were already in the database
