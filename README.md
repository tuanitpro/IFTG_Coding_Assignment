# SettlementBookingTest

## Versions

1. Basic Implementation

`BasicBookingController` provides an implementation within the controller that is complete but poorly written.

2. Mediator Implementation

`MediatorBookingController` provides an implementation using the mediator pattern and the data is stored in an [Interval Tree](https://en.wikipedia.org/wiki/Interval_tree) for O(log m + n) access.
This implementatrion also provides unit tests with a mocked repository.

## How to execute

1. Basic:

```
curl --request POST \
  --url https://localhost:5000/BasicBooking \
  --header 'Content-Type: application/json' \
  --data '{
	"bookingTime": "09:00",
	"name": "Keith"
}'
```

2. Mediator

```
curl --request POST \
  --url https://localhost:5000/MediatorBooking \
  --header 'Content-Type: application/json' \
  --data '{
	"bookingTime": "09:30",
	"name": "John"
}'
```
