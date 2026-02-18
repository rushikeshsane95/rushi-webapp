window.renderMath = () => {
    if (window.MathJax) {
        MathJax.typeset();
    } else {
        console.warn("MathJax not ready, retrying...");
        setTimeout(window.renderMath, 100); // Retry after 100ms
    }
};
