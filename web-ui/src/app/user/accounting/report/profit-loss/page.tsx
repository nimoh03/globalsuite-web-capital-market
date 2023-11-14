"use client";

import { CustomCard } from "@/app/components/customCard";
import { Button, Checkbox, DatePicker, Form, Radio, Select } from "antd";
import { observer } from "mobx-react-lite";

export default observer(function Page() {
  const [form] = Form.useForm();
  return (
    <CustomCard
      cardClassname={"mx-auto"}
      headerContent={<p>Profit/Loss Statement</p>}
    >
      <Form form={form} className="bg-gray-50 py-4 px-8 " layout="vertical">
        <div className="flex gap-3 ">
          <Form.Item
            name={"startDate"}
            className="flex-1 mb-1"
            label="From"
            rules={[{ required: true }]}
          >
            <DatePicker className="w-full" format="DD/MM/YYYY" />
          </Form.Item>

          <Form.Item
            name={"endDate"}
            className="flex-1 mb-1"
            label="To"
            rules={[{ required: true }]}
          >
            <DatePicker className="w-full" format="DD/MM/YYYY" />
          </Form.Item>

          <Form.Item
            name={"branch"}
            label={"Branch"}
            className="flex-1 mb-0"
            rules={[{ required: true }]}
          >
            <Select allowClear optionFilterProp="children"></Select>
          </Form.Item>
        </div>

        <div className="flex justify-center">
          <Form.Item
            name={"format"}
            rules={[{ required: true }]}
            initialValue={1}
            className="mb-0"
          >
            <Radio.Group>
              <Radio value={1}>Pro Format</Radio>
              <Radio value={2}>Normal</Radio>
            </Radio.Group>
          </Form.Item>
        </div>

        <div className="flex justify-center">
          <Form.Item
            name={"enrties"}
            rules={[{ required: true }]}
            className="mb-0"
          >
            <Checkbox>Exclude End Of Year Entries</Checkbox>
          </Form.Item>
        </div>

        <div className="flex justify-end">
          <Button className="bg-blue-200 text-white px-4 py-2 rounded">
            View
          </Button>
        </div>
      </Form>
    </CustomCard>
  );
});
