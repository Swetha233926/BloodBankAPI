# BloodBank API

## Overview
These are all the APIs created for the BloodBank API to get, post, update, and search donor details.

## Endpoints

### GET
- **Get all donor details**:
  Retrieves all donor details in Swagger and Postman.
- **Get donor details by ID**:
  Returns a donor's details if the ID exists. If not, a "Not Found" message is returned.

### POST
- **Add Single Donor Details**:
  Validations include:
    1. Name must not be null or empty.
    2. Age and Quantity must be greater than 0.
    3. Blood type must match one of the 8 valid types (case insensitive).
    4. Expiration date must not be earlier than the collection date.
    5. `BloodStatus` can be empty (auto-determined based on request status, expiry, or availability).

  Errors are returned for invalid entries.

- **Add Bulk Donor Details**:
  Validates each entry, adding only the correct ones while reporting errors for invalid details.

### PUT
- **Update Donor Details by ID**:
  Updates donor details if the ID exists and the details are valid. Otherwise, an error is returned.

### DELETE
- **Delete by ID**:
  Deletes a donor if the ID exists; otherwise, a "Not Found" message is returned.
- **Bulk Delete**:
  Deletes multiple donors, reporting IDs not found while proceeding with valid deletions.

### Pagination
- Supports pagination using `page` and `pageSize` parameters.

### SEARCH
- **By Blood Type**:
  Searches donors based on blood type (case insensitive).
- **By Status**:
  Searches donors by their blood status (e.g., "Available", "Requested").
- **By Name**:
  Searches donors using trimmed names (case insensitive).

## Error Handling
- Validates input details thoroughly, ensuring meaningful error messages for incorrect data.

## Swagger and Postman Integration
- Includes detailed examples for all endpoints.

