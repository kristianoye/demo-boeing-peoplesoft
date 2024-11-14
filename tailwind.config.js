/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        "./Client/**/*.{tsx,jsx,js,ts}",
        "./Views/**/*.{cshtml,html}"
    ],
    theme: {
        extend: {
            colors: {
                primary: "var(--color-primary)",
                secondary: "var(--color-secondary)",
                primaryBG: "var(--color-primaryBackground)",
                primaryText: "var(--color-primaryText)"
            }
        },
    },
    plugins: [],
}

