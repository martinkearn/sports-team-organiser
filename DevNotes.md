# Development Notes

Used as a log during development to keep track of progress and status in between sessions.

## Transaction Class
- [x] Remove Transaction.LastUpdated calculations because the built-in TimeStamp does the same thing

## Transaction Service
- [x] GetTransaction
- [x] GetTransactions with skip and take
- [x] GetTransactions with playerId
- [x] UpsertTransactionAsync
- [x] DeleteTransactionAsync
- [x] Startup DI
- [x] Refactor Transaction to only have properties, not sub-objects like PlayerEntity or Player
- [x] Put Player name in UrlSegment
- [x] Set amount back to older format (<playerurl>-<dateTime> dan-lewis-19-03-2024-19-53-23) because amount is not required given we are storing time to the second and also old data will not work
- [x] If the amount is zero, don't add a transaction at all ... negative or positive

## Pages & Components
- [x] /transactions - Show more
- [x] ShowMore as a component
- [x] /transactions/add
- [x] Game details add transaction
- [x] PAG add transaction
- [x] /transactions/{TransactionUrlSegment}
- [ ] /players/{PlayerUrlSegment}/transactions
- [ ] TransactionLink component to replace EntityTitleLink

## Current focus
