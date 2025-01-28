# Development Notes

Used as a log during development to keep track of progress and status in between sessions.

## Transaction Service
- [x] GetTransaction
- [x] GetTransactions with skip and take
- [x] GetTransactions with playerId
- [x] UpsertTransactionAsync
- [x] DeleteTransactionAsync
- [x] Startup DI
- [x] Refactor Transaction to only have properties, not sub-objects like PlayerEntity or Player
- [x] Put Player name in UrlSegment
- [ ] If the amount is zero, don't add a trasdnaction at all ... negative or positive

## Pages & Components
- [x] /transactions - Show more
- [x] ShowMore as a component
- [x] /transactions/add
- [x] Game details add transaction
- [x] PAG add transaction
- [ ] /transactions/{TransactionUrlSegment}
- [ ] /players/{PlayerUrlSegment}/transactions
- [ ] TransactionLink component to replace EntityTitleLink

## Current focus
The URL segment and labels do not appear to be always added or read consistently.

Are they being constructed properly in the Transaction class