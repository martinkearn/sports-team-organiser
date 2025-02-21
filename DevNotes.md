# Development Notes

Used as a log during development to keep track of progress and status in between sessions.

## PlayerAtGame class

PlayerAtGameEntity properties:
- [Required] public string PlayerRowKey { get; set; } = default!;
- [Required] public string GameRowKey { get; set; } = default!;
- [Required] public Enums.PlayingStatus Forecast { get; set; }
- [Required] public bool Played { get; set;} = false;
- public string UrlSegment { get; set; } = default!;
- public string Team { get; set; } = default!;

Calculated properties
- ID (from PlayerAtGameEntity.RowKey)
- LastUpdated (from PlayerAtGameEntity.Timestamp)

From Player:
- Name
- Position
- DefaultRate
- AdminRating
- Label
- Rating
- UrlSegment
- Balance
- GamesCount

From Game
- DateTime
- Label
- UrlSegment

## Tasks
- [x] Class
- [x] Service
- [] Tests
- [] Implement in pages
- [] Implement in components
- [] Remove old

## Current focus
