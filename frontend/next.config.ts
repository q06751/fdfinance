import type { NextConfig } from "next";

const apiTarget = process.env.API_URL || "http://127.0.0.1:18765";

const nextConfig: NextConfig = {
  async rewrites() {
    return [
      {
        source: "/api/:path*",
        destination: `${apiTarget}/api/:path*`,
      },
      {
        source: "/signatures/:path*",
        destination: `${apiTarget}/signatures/:path*`,
      },
    ];
  },
};

export default nextConfig;
