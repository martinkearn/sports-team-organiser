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

## Pages & Components
- [x] /transactions - Show more
- [x] ShowMore as a component
- [x] /transactions/add
- [ ] Game details add transaction
- [ ] PAG add transaction
- [ ] /transactions/{TransactionUrlSegment}
- [ ] /players/{PlayerUrlSegment}/transactions