# What Is CORS? A Simple Explanation

> **Reading time:** 10 minutes  
> **Level:** Beginner

When a frontend application calls an API, you may sometimes see an error such as:

> Access to fetch has been blocked by CORS policy.

This error is common when the frontend and backend are running on different domains, ports, or protocols.

This article explains CORS in simple English. It covers what CORS is, why browsers use it, what CORS headers do, when CORS is required, and common mistakes to avoid.

## Table of contents

1. [What does CORS mean?](#what-does-cors-mean)
2. [What is an origin?](#what-is-an-origin)
3. [Why do browsers enforce CORS?](#why-do-browsers-enforce-cors)
4. [How does CORS work?](#how-does-cors-work)
5. [What are CORS headers?](#what-are-cors-headers)
6. [What is a preflight request?](#what-is-a-preflight-request)
7. [Why is CORS required for frontend applications?](#why-is-cors-required-for-frontend-applications)
8. [When is CORS not required?](#when-is-cors-not-required)
9. [When should CORS be enabled?](#when-should-cors-be-enabled)
10. [When should CORS not be enabled?](#when-should-cors-not-be-enabled)
11. [Where should CORS be configured?](#where-should-cors-be-configured)
12. [CORS is not authentication](#cors-is-not-authentication)
13. [What about cookies and credentials?](#what-about-cookies-and-credentials)
14. [Common CORS mistakes](#common-cors-mistakes)
15. [A simple CORS example](#a-simple-cors-example)
16. [Final summary](#final-summary)

## 1. What does CORS mean?

CORS stands for **Cross-Origin Resource Sharing**.

CORS is a browser security mechanism that controls whether a web page is allowed to request and read data from another origin.

For example, imagine that your frontend application is running at:

```text
https://mywebsite.com
```

Your API is running at:

```text
https://api.mywebsite.com
```

These are different origins because their hostnames are different. The browser treats the API call as a cross-origin request.

The API must tell the browser whether requests from `https://mywebsite.com` are allowed. That permission is usually given through HTTP response headers.

## 2. What is an origin?

An origin is made up of three parts:

- Protocol, such as `http` or `https`
- Hostname, such as `example.com`
- Port, such as `80` or `5000`

These URLs have different origins:

```text
https://example.com
http://example.com
https://api.example.com
https://example.com:5000
```

Even a difference in the port number creates a different origin. For example:

```text
http://localhost:3000
http://localhost:5000
```

Both applications run on `localhost`, but they use different ports. Therefore, they have different origins.

## 3. Why do browsers enforce CORS?

Browsers follow an important security rule called the **same-origin policy**.

This rule prevents one website from freely reading private information from another website.

Imagine that you are logged in to your bank website. At the same time, you visit a malicious website in another browser tab.

Without browser security rules, the malicious website might try to call your bank's API and read your account information using your existing login cookies.

The same-origin policy helps prevent this kind of data theft.

CORS provides a controlled way for a server to say:

> I trust this other website and allow it to read my responses.

CORS is therefore not mainly a frontend feature. It is a permission given by the server, and the browser enforces that permission.

For more detail, see the [MDN CORS documentation](https://developer.mozilla.org/en-US/docs/Web/HTTP/Guides/CORS).

## 4. How does CORS work?

Suppose a frontend application sends a request from:

```text
https://mywebsite.com
```

to an API at:

```text
https://api.mywebsite.com
```

The browser adds an `Origin` header to the request:

```http
Origin: https://mywebsite.com
```

The API can respond with:

```http
Access-Control-Allow-Origin: https://mywebsite.com
```

The browser sees this response header and allows the frontend to read the response.

If the API does not return the correct CORS header, the browser blocks the frontend from reading the response.

The request may still reach the server. CORS does not necessarily stop the request from being sent. It mainly controls whether browser JavaScript is allowed to read the response.

## 5. What are CORS headers?

CORS headers are normal HTTP headers that describe which cross-origin requests are allowed.

### 5.1 `Access-Control-Allow-Origin`

This header specifies which origin is allowed to access the API.

```http
Access-Control-Allow-Origin: https://mywebsite.com
```

You can also use:

```http
Access-Control-Allow-Origin: *
```

This means that requests from any origin are allowed to read the response.

Using `*` may be acceptable for a genuinely public, read-only API. It is usually not suitable for private APIs or APIs that use cookies and other credentials.

In production, it is normally safer to allow only known frontend applications:

```http
Access-Control-Allow-Origin: https://app.mycompany.com
```

### 5.2 `Access-Control-Allow-Methods`

This header specifies which HTTP methods are allowed.

```http
Access-Control-Allow-Methods: GET, POST, PUT, DELETE
```

For example, this tells the browser that the frontend may use `GET`, `POST`, `PUT`, and `DELETE`.

### 5.3 `Access-Control-Allow-Headers`

This header specifies which request headers the frontend is allowed to send.

```http
Access-Control-Allow-Headers: Content-Type, Authorization
```

This is often needed when the frontend sends:

- JSON using the `Content-Type` header
- A bearer token using the `Authorization` header
- Custom application headers

### 5.4 `Access-Control-Allow-Credentials`

This header allows the browser to send credentials such as cookies or authentication information.

```http
Access-Control-Allow-Credentials: true
```

When credentials are allowed, the server cannot use:

```http
Access-Control-Allow-Origin: *
```

Instead, it must specify an exact origin:

```http
Access-Control-Allow-Origin: https://mywebsite.com
Access-Control-Allow-Credentials: true
```

Credentials should be enabled only when they are actually required.

### 5.5 `Access-Control-Expose-Headers`

Browsers allow frontend JavaScript to read only certain response headers by default.

If the frontend needs to read a custom response header, the server must expose it:

```http
Access-Control-Expose-Headers: X-Total-Count
```

### 5.6 `Access-Control-Max-Age`

This header tells the browser how long it can remember the result of a preflight request.

```http
Access-Control-Max-Age: 600
```

This means that the browser may cache the preflight result for 600 seconds.

### 5.7 Request-side CORS headers

The browser may include these request headers:

```http
Origin: https://mywebsite.com
```

For a preflight request, it may also include:

```http
Access-Control-Request-Method: POST
Access-Control-Request-Headers: content-type, authorization
```

These headers tell the API what the browser wants to do before it sends the actual request.

## 6. What is a preflight request?

Some cross-origin requests are more complicated or potentially more sensitive than normal requests.

Before sending such a request, the browser may send an `OPTIONS` request first. This is called a **preflight request**.

For example, the frontend wants to send:

```http
POST /orders HTTP/1.1
Origin: https://mywebsite.com
Content-Type: application/json
Authorization: Bearer token
```

Before sending the actual `POST` request, the browser may send:

```http
OPTIONS /orders HTTP/1.1
Origin: https://mywebsite.com
Access-Control-Request-Method: POST
Access-Control-Request-Headers: content-type, authorization
```

The API should respond with permission:

```http
Access-Control-Allow-Origin: https://mywebsite.com
Access-Control-Allow-Methods: POST
Access-Control-Allow-Headers: Content-Type, Authorization
```

If the response is acceptable, the browser sends the actual `POST` request.

The preflight request is like the browser asking:

> If I send this type of request from this website, will you accept it?

### 6.1 Which requests usually cause a preflight?

A preflight is commonly required when the request:

- Uses methods such as `PUT`, `PATCH`, or `DELETE`
- Sends JSON using `Content-Type: application/json`
- Sends an `Authorization` header
- Sends custom headers
- Uses other non-simple request settings

A simple `GET` request usually does not require a preflight. However, the API still needs to return the correct `Access-Control-Allow-Origin` header if the frontend wants to read the response.

## 7. Why is CORS required for frontend applications?

Consider a single-page application built with React, Angular, Vue, or Blazor WebAssembly.

The application runs inside the user's browser and calls an API directly:

```text
Browser frontend → API
```

For example:

```text
https://frontend.example.com
          |
          | fetch()
          v
https://api.example.com
```

Because the frontend and API have different origins, the browser applies CORS rules.

The API must allow the frontend origin:

```http
Access-Control-Allow-Origin: https://frontend.example.com
```

Without this permission, the API might work perfectly when called through Postman or cURL, but fail when called from browser JavaScript.

Postman and cURL do not enforce browser CORS rules. CORS is primarily a browser behavior.

## 8. When is CORS not required?

CORS is not required when the API is not being called directly by browser JavaScript from another origin.

For example:

```text
Browser → Backend application → External API
```

In this design, the browser calls its own backend. The backend then calls the external API.

The external API does not need CORS for this call because it is a server-to-server request. Servers do not enforce browser CORS rules.

CORS is also usually not required when the frontend and backend use the same origin:

```text
https://mywebsite.com
https://mywebsite.com/api
```

Even if the frontend and API are separate applications internally, a reverse proxy can expose them under the same public origin:

```text
https://mywebsite.com       → frontend
https://mywebsite.com/api   → backend API
```

From the browser's point of view, these requests have the same origin, so CORS is not needed.

## 9. When should CORS be enabled?

CORS should be enabled when a browser-based frontend must directly call an API hosted on a different origin.

Typical examples include:

- A React frontend calling a .NET API
- A Vue application calling a Node.js API
- A frontend hosted on a CDN calling an API hosted elsewhere
- A local development frontend calling a local API on another port

For example:

```text
Frontend: http://localhost:3000
API:      https://localhost:7000
```

The API must allow the frontend's origin during development.

In production, it is better to allow only the actual production frontend:

```http
Access-Control-Allow-Origin: https://app.example.com
```

## 10. When should CORS not be enabled?

CORS does not need to be enabled simply because an API is public.

For example, if an API is used only by:

- Other backend services
- Scheduled jobs
- Command-line applications
- Mobile applications using native HTTP clients
- Internal server processes

then browser CORS may not be relevant.

A native mobile application is not the same as a website running inside a browser. It usually does not follow browser CORS restrictions.

You should also avoid enabling CORS for every origin unless there is a clear reason:

```http
Access-Control-Allow-Origin: *
```

This may be acceptable for a genuinely public, read-only API. It is usually a poor choice for private APIs, administrative APIs, or APIs that use cookies.

## 11. Where should CORS be configured?

CORS should be configured in the component that controls the API response.

This could be:

- The API application
- A web server such as IIS or Nginx
- An API gateway
- A reverse proxy
- A cloud load balancer

For many applications, configuring CORS in the API framework is the clearest approach because the API owns the rules.

It can also be configured at the web server or API gateway level. This can be useful when many APIs share the same CORS policy or when the gateway owns the external traffic rules.

The important thing is to avoid confusing or conflicting configurations. If the API adds one CORS header and the proxy adds another, the browser may reject the response.

CORS headers should also be returned for preflight responses and, where appropriate, error responses. Otherwise, the frontend may see only a generic CORS error instead of the real API error.

## 12. CORS is not authentication

CORS does not decide whether a user is logged in.

It does not replace:

- Authentication
- Authorization
- Access tokens
- API keys
- Role checks
- Input validation
- Rate limiting

CORS only controls whether browser JavaScript from one origin can read the response.

For example, this configuration:

```http
Access-Control-Allow-Origin: https://mywebsite.com
```

does not mean that every user of `mywebsite.com` is authorized to access all API data.

The API must still authenticate the user and check whether the user has permission to access the requested resource.

## 13. What about cookies and credentials?

Some APIs use cookies for authentication.

If a frontend on another origin must send cookies, both the frontend and backend must be configured correctly.

The frontend may need to send credentials:

```javascript
fetch("https://api.example.com/profile", {
  credentials: "include"
});
```

The API must respond with:

```http
Access-Control-Allow-Origin: https://frontend.example.com
Access-Control-Allow-Credentials: true
```

The wildcard origin cannot be used with credentials:

```http
Access-Control-Allow-Origin: *
Access-Control-Allow-Credentials: true
```

This combination is invalid.

Cookies also have additional security rules, such as `SameSite`, `Secure`, and domain restrictions. Therefore, enabling CORS alone may not be enough to make cookie authentication work.

When cookies are used across origins, protection against cross-site request forgery, commonly called CSRF, should also be considered.

## 14. Common CORS mistakes

### 14.1 Allowing every origin

This configuration is often copied during development:

```http
Access-Control-Allow-Origin: *
```

It may hide the real problem, and it may expose data more broadly than intended. Use a specific allowlist in production.

### 14.2 Adding CORS headers to the frontend

CORS permission must come from the server. Adding this header to a browser request does not solve the problem:

```http
Access-Control-Allow-Origin: *
```

This is a response header, not a request header. The API must return it.

### 14.3 Forgetting the `OPTIONS` request

The actual `GET` or `POST` request may be configured correctly, but the browser may send an `OPTIONS` preflight request first.

If the server does not handle `OPTIONS`, the browser may stop before sending the real request.

### 14.4 Allowing the wrong origin

These are different origins:

```text
http://localhost:3000
https://localhost:3000
http://localhost:3001
```

The protocol and port must match exactly.

### 14.5 Using `*` with credentials

This is not allowed:

```http
Access-Control-Allow-Origin: *
Access-Control-Allow-Credentials: true
```

Use the exact frontend origin instead.

### 14.6 Adding duplicate CORS headers

If both the API and the reverse proxy add CORS headers, the response may contain duplicate values. This can cause the browser to reject the response.

### 14.7 Treating CORS as an authentication system

CORS should never be used instead of authentication and authorization.

### 14.8 Testing only with Postman

Postman may successfully call an API even when a browser frontend cannot. Always test browser-based calls from the actual frontend as well.

## 15. A simple CORS example

A frontend at:

```text
https://frontend.example.com
```

calls an API at:

```text
https://api.example.com
```

The API may return:

```http
Access-Control-Allow-Origin: https://frontend.example.com
Access-Control-Allow-Methods: GET, POST
Access-Control-Allow-Headers: Content-Type, Authorization
```

If the frontend needs to send cookies:

```http
Access-Control-Allow-Origin: https://frontend.example.com
Access-Control-Allow-Credentials: true
```

The exact configuration depends on the API framework and authentication method.

## 16. Final summary

CORS is a browser security mechanism that controls cross-origin requests.

CORS is needed when:

- A frontend runs in a browser
- The frontend calls an API directly
- The frontend and API have different origins

CORS is usually not needed when:

- The API is called by another backend
- The API is called by a command-line tool
- The frontend and API use the same origin
- A server handles the external API call

CORS should be configured carefully. In most cases, allow only trusted frontend origins and only the methods and headers that are required.

Most importantly, remember that CORS is not authentication. It does not secure an API by itself. It tells browsers which websites are allowed to read API responses.

## Further reading

- [MDN: Cross-Origin Resource Sharing](https://developer.mozilla.org/en-US/docs/Web/HTTP/Guides/CORS)
- [WHATWG Fetch Standard: CORS protocol](https://fetch.spec.whatwg.org/#http-cors-protocol)
