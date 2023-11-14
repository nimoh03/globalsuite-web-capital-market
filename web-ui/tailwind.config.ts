import type { Config } from "tailwindcss";
const plugin = require("tailwindcss/plugin");

const config: Config = {
  important: "#app",
  content: [
    "./src/pages/**/*.{js,ts,jsx,tsx,mdx}",
    "./src/components/**/*.{js,ts,jsx,tsx,mdx}",
    "./src/app/**/*.{js,ts,jsx,tsx,mdx}",
  ],
  theme: {
    extend: {
      fontFamily: {
        sans: ["Manrope", "Inter", "sans-serif"],
      },
      backgroundImage: {
        "gradient-radial": "radial-gradient(var(--tw-gradient-stops))",
        "gradient-conic":
          "conic-gradient(from 180deg at 50% 50%, var(--tw-gradient-stops))",
      },
      colors: {
        blue: {
          100: "#0436E7",
          200: "#194BFB",
          300 : '#436CFF',
          
        },
        red : {
          150 : '#DD3333',
          250 :'#FEE2E2'
        },
        gray: {
          40:'#E8EDFF',
          50: "#FAFAFA",
          100 : '#EDF2F7',
          300: "#E2E8F0",
          400: "#CBD5E0",
          600: "#718096",
          700: "rgba(216, 227, 248, 0.20)",
          900: "#1A202C",
          500: '#5D6A83'
        },
        green: {
          100: "#F6FDF9",
          200 : '#128A3E'
        },
        orange: {
          100: "#FF784B",
        },
        yellow: {
          100: "#FFFCF0",
        },
        "alert-success": "#22C55E",
        "alert-error-light": "#FEE2E2",
        "alert-warning": "#FACC15",
      },
    },
  },
  plugins: [
    plugin(function ({ addBase, addComponents, addUtilities, theme }: any) {
      addBase({
        h1: {
          fontSize: theme("fontSize.4xl"),
        },
        h2: {
          fontSize: theme("fontSize.3xl"),
        },
        h3: {
          fontSize: theme("fontSize.2xl"),
          fontWeight: 600,
        },
        h4: {
          fontSize: theme("fontSize.xl"),
          fontWeight: 600,
        },
      });
    }),
  ],
};
export default config;
