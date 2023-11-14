import type { ThemeConfig } from "antd";

export const antdTheme: ThemeConfig = {
  token: {
    // fontSize: 16,
    colorPrimary: "#194BFB",
    fontFamily: "Manrope, Inter, sans-serif",
    controlHeightLG: 56,
    controlHeight: 40,
    paddingContentHorizontalLG: 0,
    screenSMMax: 600,
  },
  components: {
    Layout: {
      colorBgContainer: "white",
      colorBgBody: "white",
    },

    Button: {},
    Menu: {
      itemHeight: 50,
      itemColor: "#000",
      horizontalLineHeight: "50px",
    },
    Select: {
      colorText: "#718096",
    },
    Table: {
      headerBg: "#B2BDE8",
      cellFontSize: 12,
      headerBorderRadius: 0,
    },
    Tag: {
      paddingXXS: 2,
    },
  },
};
