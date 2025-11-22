# Changelog

All notable changes to the GoalGrow project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [Unreleased]

### Added
- User Management endpoints implementation
  - GET /api/users/me - Get current user profile
  - PUT /api/users/me - Update current user profile
  - GET /api/users/{id} - Get user by ID (Admin only)
  - GET /api/users/me/wallet - Get investor wallet details
  - GET /api/users/me/accounts - Get user's bank accounts
  - DELETE /api/users/me - GDPR right to be forgotten
  
- Admin Dashboard endpoints
  - GET /api/admin/users - Paginated user list with search and filters
  - GET /api/admin/stats - Platform-wide user statistics
  - PUT /api/admin/users/{id}/status - User activation/deactivation
  - DELETE /api/admin/users/{id} - Admin-initiated user deletion

- New DTOs
  - WalletResponse - Wallet information for investors
  - AccountSummaryResponse - Bank account details
  - UserStatsResponse - Platform statistics
  - UserListResponse - Paginated user list

- Security features
  - Account number masking (shows only last 4 digits)
  - GDPR-compliant soft delete via data anonymization
  - Role-based authorization (investor, consultant, admin)

### Changed
- Extended IUserService interface with 5 new methods
- Organized UserService.cs with regions for better code readability

### Fixed
- N/A

### Security
- Implemented GDPR right to be forgotten with data anonymization
- Added account number masking for security

---

## [0.1.0-alpha] - 2025-01-18

### Added
- Initial project setup
- Database schema with EF Core migrations
- Keycloak authentication integration
- Basic API structure (Health endpoint)
- User synchronization with Keycloak
- TPH (Table-Per-Hierarchy) implementation for User types
- Comprehensive documentation

### Database
- Created GoalGrowDb schema
- 13 unique indexes for data integrity
- 15 composite indexes for query optimization
- Database performance score: 95/100

### Documentation
- Complete setup guide
- Architecture documentation
- Database audit report
- Business requirements document
- Development roadmap

---

[Unreleased]: https://github.com/EdoardoCarollo99/GoalGrow/compare/v0.1.0-alpha...HEAD
[0.1.0-alpha]: https://github.com/EdoardoCarollo99/GoalGrow/releases/tag/v0.1.0-alpha
