# Product Definition

## Purpose

Sellow is a multi-category marketplace where users can buy and sell products.

It is being developed as a solo portfolio project, with the possibility of a future public release.

## Users

A single account allows a user to act as both a buyer and a seller.

### Buyers

Buyers can:

- Search and filter offers across multiple categories.
- Choose quantities and add products to a cart.
- Place orders and select available payment and delivery methods.
- Track their orders and confirm receipt.
- Rate their purchase experience.

### Sellers

Sellers can:

- Publish offers and specify available quantities.
- Configure the payment and delivery methods available for their offers.
- Receive notifications about new orders.
- Process orders and mark them as shipped.

## Purchase Flow

1. A seller publishes an offer.
2. A buyer finds the offer and selects a quantity.
3. The buyer adds the product to their cart.
4. The buyer checks out with one seller, selecting payment and delivery methods.
5. The application creates an order and reserves the ordered quantities.
6. For upfront payment, the order awaits payment confirmation.
   For cash on delivery, the order becomes ready for processing immediately.
7. The seller receives a notification about the new order, including whether
   it is ready for processing.
8. The seller begins processing the order.
9. The seller ships the order and marks it as shipped.
10. The buyer confirms receipt.
11. The buyer can rate the purchase experience.

## Business Rules

### Offers and Quantities

- Sellow supports offers across multiple product categories.
- An offer can contain multiple units of a product.
- A buyer can purchase multiple units from the same offer.
- A purchase must not exceed the quantity available for sale.
- Concurrent purchases must not result in overselling.

### Cart and Orders

- A cart can contain products from multiple sellers.
- Checkout is performed separately for each seller.
- Each order belongs to one buyer and one seller.
- An order can contain multiple offers from the same seller.
- Each order has its own payment, delivery, and purchase rating.

### Stock Reservation

- Adding a product to a cart does not reserve stock.
- Placing an order reserves the ordered quantities.
- For upfront payment, an unpaid order is cancelled when its payment
  deadline expires, and the reserved quantities are released.
- For cash on delivery, the ordered quantities remain allocated
  while the order is being fulfilled.

### Payments

- Supported payment options are upfront payment and cash on delivery.
- Buyers can only select payment methods enabled by the seller.
- Orders paid upfront become ready for processing after payment confirmation.
- Cash-on-delivery orders can be processed before payment is collected.
- Payment status is tracked separately from order status.

### Delivery Confirmation

- In the initial version, the buyer confirms receipt manually.
- Elapsed time since shipment does not constitute delivery confirmation.

### Purchase Ratings

- A buyer can rate a purchase after confirming receipt.
- Each order can receive one rating, regardless of the number of items or units.
- A rating consists of a score from 1 to 5 and an optional comment.
- The rating represents the overall purchase experience with the seller.
- Separate product reviews are outside the initial scope.

## Open Questions

### Product and Offer Structure

- Will sellers create independent listings, or attach offers to products
  in a shared catalog?
- How will categories and category-specific attributes be managed?

### Payments and Reservations

- How long should buyers have to complete upfront payment?
  Thirty minutes is an initial proposal, not a confirmed requirement.
- Will the portfolio version simulate payments or use a provider's sandbox?
- How should failed payments and payments confirmed after reservation
  expiry be handled?

### Order Lifecycle

- When and by whom can an order be cancelled?
- How should unfulfilled orders, returns, and refunds be handled?
- What happens if a buyer never confirms receipt?
- How will delivery options, costs, and shipment details be represented?
- Which notification channels will be supported?

### Potential Future Features

- Selling services in addition to products.
- Allowing sellers to rate buyers.
- Integrating shipment tracking.
- Supporting separate product reviews.