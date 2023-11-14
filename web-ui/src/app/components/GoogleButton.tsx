import { images } from "@/theme";
import { Button } from "antd";
import Image from "next/image";

export const GoogleButton = () => {
  return (
    <Button
      icon={<Image src={images.googleIcon} alt="google icon" />}
      className="flex gap-2 justify-center text-gray-900"
      size="large"
    >
      Sign In with Google
    </Button>
  );
};
