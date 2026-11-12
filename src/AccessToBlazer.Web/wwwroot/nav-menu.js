// Collapses the mobile nav menu after a link is clicked. This was an inline
// onclick attribute, which a script-src 'self' Content-Security-Policy blocks.
// Delegated on document so it survives Blazor's enhanced navigation.
document.addEventListener("click", (event) => {
  if (event.target instanceof Element && event.target.closest(".nav-scrollable")) {
    document.querySelector(".navbar-toggler")?.click();
  }
});
