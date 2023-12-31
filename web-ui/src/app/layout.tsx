"use client";

import "../theme/styles.scss";
import type { Metadata } from "next";
import { Manrope } from "next/font/google";
import { RootProvider, stores } from "../stores/root-context";
import StyledComponentsRegistry from "../lib/AntdRegistry";
import { ToastContainer } from "react-toastify";

import "react-toastify/dist/ReactToastify.css";
import { ConfigProvider } from "antd";
import { antdTheme } from "@/theme";

const manRope = Manrope({
  subsets: ["latin"],
  display: "swap",
  variable: "--font-manrope",
});

// export const metadata: Metadata = {
//   title: "Create Next App",
//   description: "Generated by create next app",
// };

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <RootProvider value={stores}>
      <ConfigProvider theme={antdTheme}>
        <html lang="en" id="app" className="text-gray-900">
          <body className={manRope.variable}>
            <StyledComponentsRegistry>{children}</StyledComponentsRegistry>
            <ToastContainer />
          </body>
        </html>
      </ConfigProvider>
    </RootProvider>
  );
}
