# Domain-Driven Design — Real World Example 

This document is an original summary and explanation of the key ideas and examples from the referenced article on Domain-Driven Design (DDD). It captures the main concepts, patterns, and a concrete example (order placement) while rephrasing the original content.

## What is DDD?

Domain-Driven Design (DDD) is an approach that centers software design around a deep understanding of the business domain. Instead of letting infrastructure, controllers, or database schemas dictate the code structure, DDD encourages designing models that reflect business concepts, invariants, and behaviors.

The core idea: code should express the business. When business rules change, changes should primarily affect domain objects rather than scattered utilities or service layers.

## Traditional (layered) development vs DDD

Traditional layered architectures separate concerns into controllers, services, and DAOs/Repositories. Often the domain objects become anemic (data holders) and business rules live in services or utilities.

Problems with that approach:
- Business rules end up scattered across multiple layers.
- Domain objects become passive DTOs without behavior.
- Changing a business rule requires hunting through services, controllers, and utils.

DDD advocates a richer domain model:
- Entities and value objects encapsulate relevant business logic and invariants.
- Domain services represent operations that span multiple entities.
- Application services orchestrate use-cases (repositories + domain services + infrastructure) but contain little or no domain logic.

## Key DDD Concepts

- Aggregate Root: an entity that controls a consistency boundary. All modifications to related objects should go through the aggregate root to preserve invariants.

- Domain (vs Application) Service: a domain service holds business logic that cannot reasonably belong to a single entity or value object (for example, transferring funds between accounts). Application services coordinate a use-case, retrieving aggregates and calling domain services.

- Domain Events: explicit objects that represent important business state changes (e.g., `OrderCreated`, `UserRegistered`). They capture intent and can be published to other parts of the system.

## Small examples (paraphrased)

Traditional controller/service flow (simplified):

```
// Application service orchestrates the flow
Account from = accountRepository.findById(fromId);
Account to = accountRepository.findById(toId);
transferService.transfer(from, to, new Money(amount));
messageQueue.send(new TransferEvent(...));
```

In a DDD view, `transferService` is a domain service that executes the core business rule (debit/credit) while `Account` entities encapsulate debit/credit invariants.

Aggregate root example (addresses under user):

```
class User { 
  private List<Address> addresses;
  public void addAddress(Address a) {
    if (addresses.size() >= 5) throw new AddressLimitExceeded();
    addresses.add(a);
  }
}
```

Here the `User` aggregate root enforces the rule limiting addresses.

## Rich model vs Anemic model (registration example)

- Anemic: validation and invariants live in service or utilities; the `User` object is a simple data holder.
- Rich model: `User` constructor or methods validate passwords and enforce invariants, e.g.:

```
class User {
  public User(String username, String password) {
    if (!isValidPassword(password)) throw new InvalidPassword();
    this.password = encrypt(password);
  }
}
```

Putting rules close to the data they protect improves cohesion and reduces scatter.

## Concrete DDD Example: Placing an E-commerce Order

Problem: When placing an order the system must validate stock, apply coupons, calculate totals, and persist the order.

Traditional (service-heavy) implementation often mixes stock checks, price calculations, coupon logic, and persistence in one bloated service. This leads to brittle code and scattered rules.

DDD approach (rich model):
- Create an `Order` aggregate that encapsulates order-level invariants and calculations.
- `OrderItem` value objects check stock availability.
- `Money` value object handles precise money arithmetic.
- Coupon rules live in `Coupon` or `Order` methods (e.g., `validateCoupon`).

Example (paraphrased):

```
class Order {
  public Order(User user, List<OrderItem> items, Coupon coupon) {
    items.forEach(i -> i.checkStock());
    this.total = items.map(OrderItem::subtotal).reduce(Money.ZERO, Money::add);
    if (coupon != null) { validateCoupon(coupon, user); total = coupon.apply(total); }
  }
}

class OrderService { 
  public Order createOrder(...) {
    Order order = new Order(...);
    orderRepository.save(order);
    domainEventPublisher.publish(new OrderCreatedEvent(order));
    return order;
  }
}
```

Benefits: stock rules and coupon logic are owned by the domain, so when requirements change you modify domain code instead of multiple service/util locations.

## When to Use DDD

- DDD is most valuable for complex domains with evolving business rules (e-commerce, finance, ERP).
- Avoid DDD for simple CRUD applications or small admin panels — it can be overengineering.

Use DDD when: changing business rules should ideally require only domain-level changes and not rip through controllers and DAOs.

## Practical Guidance

- Keep application services thin: they orchestrate repositories, domain services, and side-effects (messaging, persistence), but should not contain domain rules.
- Prefer meaningful domain objects (entities, value objects) that encapsulate behavior and invariants.
- Use domain events to make business state changes explicit and decoupled from infrastructure.

## References
 
- Inspired by [Domain-Driven Design Explained: A Real World Example](https://dev.to/leapcell/domain-driven-design-explained-a-real-world-example-581j) . This document is an original summary and does not reproduce the original text verbatim.


