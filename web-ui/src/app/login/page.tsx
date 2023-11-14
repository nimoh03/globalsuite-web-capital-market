"use client";

import { images } from "@/theme";
import { Button, Checkbox, Form, Input, Layout } from "antd";
import { observer } from "mobx-react-lite";
import Image from "next/image";
import { GoogleButton } from "../components/GoogleButton";
import { AppleButton } from "../components/AppleButton";
import { ROUTES } from "@/lib/routes";
import { useEffect, useState } from "react";
import { useStores } from "@/hooks/use-store";
import { RETURN_URL_KEY } from "@/lib/constants";

export default observer(function Login() {
  const { authStore } = useStores();

  const [form] = Form.useForm();
  // Watch all values
  const values = Form.useWatch([], form);
  const [submittable, setSubmittable] = useState(false);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    form.validateFields({ validateOnly: true }).then(
      () => {
        setSubmittable(true);
      },
      () => {
        setSubmittable(false);
      }
    );
  }, [values]);

  const submit = async (values: any) => {
    setLoading(true);

    const response = await authStore.login(values);

    setLoading(false);

    if (response?.kind === "ok") {
      const returnUrl = sessionStorage.getItem(RETURN_URL_KEY);

      window.location.href = returnUrl || ROUTES.dashboard().path;
      sessionStorage.removeItem(RETURN_URL_KEY);
    }
  };

  return (
    <Layout className="flex-row h-full">
      <div className="flex flex-col items-start flex-1 h-full  py-12 pl-12">
        <Image src={images.logo} alt="logo" className="h-14 object-contain" />

        <div className="flex flex-col items-center  m-auto gap-7 w-[400px]">
          <h3 className="max-w-[300px] text-center">Log in to Globalsuite</h3>
          {/* <div className="flex gap-4 ">
            <GoogleButton />
            <AppleButton />
          </div> */}

          {/* <div className="flex w-full items-center  ">
            <span className="flex-1 bg-gray-300 h-[1px]"></span>
            <span className="text-gray-600 mx-4">
              Or with Username/Password
            </span>
            <span className="flex-1 bg-gray-300 h-[1px]"></span>
          </div> */}

          <Form
            form={form}
            className="w-full max-w[400px]"
            onFinish={submit}
            disabled={loading}
          >
            <Form.Item
              name={"username"}
              className="mb-3"
              rules={[{ required: true }]}
            >
              <Input placeholder="Username" size="large" />
            </Form.Item>

            <Form.Item name="password" rules={[{ required: true }]}>
              <Input.Password placeholder="Password" size="large" />
            </Form.Item>

            <div className="flex w-full justify-between">
              <Form.Item name="remember" valuePropName="checked">
                <Checkbox className="font-medium">Remember me</Checkbox>
              </Form.Item>

              <a href="#" className="text-blue-500 font-medium">
                Forgot Password?
              </a>
            </div>

            <Button
              htmlType="submit"
              className={`w-full ${
                submittable ? "bg-blue-200" : "bg-gray-300"
              }  text-white font-bold`}
              size="large"
              type="primary"
              disabled={!submittable}
              loading={loading}
            >
              Sign in
            </Button>
          </Form>
        </div>
      </div>

      <div className="flex justify-end flex-1 h-full py-4 pr-4">
        <div className="h-full w-full overflow-hidden relative">
          <Image
            src={images.loginRightSidebar}
            alt="right sidebar"
            className="absolute top-0 -right-40 h-full object-contain"
          />
        </div>
      </div>
    </Layout>
  );
});
